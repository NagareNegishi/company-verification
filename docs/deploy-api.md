# API Deployment Plan

**Status:** Render GitHub App connected to repo. Remaining steps below.

- Add a production `Dockerfile` to the repo root (Render does not support .NET
  natively; Docker is required).
- Fix `UseHttpsRedirection()` in `Program.cs` — make it development-only. Render
  terminates TLS at their proxy; the app receives plain HTTP.
- Create the Render web service: point it at this repo, branch main, runtime
  Docker, auto-deploy set to "After CI Checks Pass". Add `NZBN__SubscriptionKey`
  and `ABR__Guid` as environment variables in the Render dashboard.
- Write `.github/workflows/deploy.yml`: triggers on push to main, runs
  `dotnet test`. No deploy hook or secret needed. Render picks up the passing
  CI check and deploys automatically.

The Dockerfile and Program.cs fix must land on main before the first deploy will
succeed.
