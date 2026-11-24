#!/bin/bash
# Covers the gap: deny rules don't reach files written by subprocesses.

INPUT=$(cat)
CMD=$(echo "$INPUT" | jq -r '.tool_input.command // empty')

if echo "$CMD" | grep -Eq '(python3?|node|ruby|perl)[[:space:]]+-[ce][[:space:]]'; then
  echo "Blocked: interpreter one-liners can write source files. Explain it instead; I type it." >&2
  exit 2
fi

if echo "$CMD" | grep -Eq '(tee|sed[[:space:]]+-i)[^|;&]*\.(cs|csproj|sln|sql)'; then
  echo "Blocked: that writes a source file. Tell me the change; I make it." >&2
  exit 2
fi

exit 0
