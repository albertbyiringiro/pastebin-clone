#!/bin/bash
cd "$CLAUDE_PROJECT_DIR" 2>/dev/null || exit 0
echo "Open ticket:"
grep -n "DOING" TICKETS.md 2>/dev/null | head -3
echo "Last commits:"
git log --oneline -3 2>/dev/null
echo "Recent confusions:"
tail -4 LEARNING.md 2>/dev/null
exit 0

