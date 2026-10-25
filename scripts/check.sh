#!/usr/bin/env bash
# Checks everything the course needs, and says what to do for each thing that is missing.
# Run it from anywhere: ./scripts/check.sh
#
# No `set -e` on purpose: a junior with three problems should see all three at once,
# not fix one, run again, and meet the next.

OLLAMA_URL="${OLLAMA_URL:-http://localhost:11434}"
MODEL="${MODEL:-gemma4:e2b}"
failed=0

ok()   { printf '  ok    %s\n' "$1"; }
fail() { printf '  FAIL  %s\n        fix: %s\n' "$1" "$2"; failed=1; }

echo "Checking the course setup..."

if command -v node >/dev/null; then
  major=$(node --version | sed 's/^v//' | cut -d. -f1)
  if [ "$major" -ge 24 ]; then ok "node $(node --version)"
  else fail "node $(node --version) is too old, need 24+" "brew upgrade node"; fi
else
  fail "node is not installed" "brew install node"
fi

if command -v dotnet >/dev/null; then
  ver=$(dotnet --version)
  if [ "${ver%%.*}" -ge 10 ]; then ok "dotnet $ver"
  else fail "dotnet $ver is too old, need 10+" "brew upgrade dotnet"; fi
else
  fail "dotnet is not installed" "brew install dotnet"
fi

if ! command -v ollama >/dev/null; then
  fail "ollama is not installed" "brew install ollama"
elif ! version=$(curl -s -m 3 "$OLLAMA_URL/api/version"); then
  fail "the model server is not running at $OLLAMA_URL" "run 'ollama serve' in another terminal"
else
  ok "ollama server $version"

  # `ollama list` prints "gemma4:e2b  <id>  7.5 GB ...". Match the name and a space, so that
  # gemma4:e2b does not also match gemma4:e2b-something.
  if ollama list | grep -q "^$MODEL "; then
    ok "model $MODEL is pulled"

    # The one check that matters most: chapters 05+ need the model to return a tool call as
    # data, not as prose. The first call can take a while — the model loads into RAM.
    echo "  ..    asking $MODEL for a tool call (the first time can take a minute)"
    reply=$(curl -s -m 300 "$OLLAMA_URL/api/chat" -d '{
      "model": "'"$MODEL"'", "stream": false,
      "messages": [{"role":"user","content":"List the files in the current folder."}],
      "tools": [{"type":"function","function":{
        "name":"list_files","description":"List files in a folder",
        "parameters":{"type":"object","properties":{"path":{"type":"string"}},
                      "required":["path"]}}}]
    }')
    if echo "$reply" | grep -q '"tool_calls"'; then ok "$MODEL can call tools"
    else fail "$MODEL answered in prose, not with a tool call" \
              "use gemma4:e2b, or read chapter 04 (tools by hand)"; fi
  else
    fail "model $MODEL is not pulled" "ollama pull $MODEL"
  fi
fi

echo
if [ "$failed" -eq 0 ]; then echo "All good. You are ready for chapter 01."
else echo "Fix the FAIL lines above, then run this again."; exit 1; fi
