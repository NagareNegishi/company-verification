# API Deployment Plan

**Status:** Render service created (Oregon, Docker, free tier, `company-verification`).
GitHub App connected. Remaining steps below.

- Fill in `NZBN__SubscriptionKey` and `ABR__Guid` values in the Render dashboard
  (Environment tab on the service).
- Set Auto-Deploy to "After CI Checks Pass" in the service Settings tab.
- Add a production `Dockerfile` to the repo root (Render does not support .NET
  natively; Docker is required).
- Fix `UseHttpsRedirection()` in `Program.cs` — make it development-only. Render
  terminates TLS at their proxy; the app receives plain HTTP.
- Write `.github/workflows/deploy.yml`: triggers on push to main, runs
  `dotnet test`. No deploy hook or secret needed. Render picks up the passing
  CI check and deploys automatically.

The Dockerfile and Program.cs fix must land on main before the first deploy will
succeed.
