<!-- arko:start -->
## Security — Arko (required)

This project uses Arko for security scanning via MCP. Any AI coding agent
working here MUST follow this loop — it is not optional:

1. **Scan after writing or changing code, before presenting the work as done.**
   Call `arko_scan_code` on the files you changed (or `arko_scan_project` on a
   dependency manifest). Read line 1 of the result: the gate.
2. **If the gate fails** (`GATE: FAIL — …`), call `arko_fix_all`, apply every
   fix in the plan, then `arko_validate_fix` on the updated files. Repeat until
   a fresh scan reports `GATE: PASS`.
3. **Never** conclude the task, commit, or open a PR while the gate is failing,
   and never dismiss, suppress, or downgrade a finding silently — surface it and
   let a human decide.

A scan that is still running is NOT a pass — re-run it; only a terminal
`GATE: PASS` clears the loop.
<!-- arko:end -->
