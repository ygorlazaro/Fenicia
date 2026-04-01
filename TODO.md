# Fix Authentication Scheme Error in Controllers

## Steps to Complete

- [x] 1. Create reusable `ForbidWithMessage` helper in Fenicia.Common.Api/Controllers/ControllerBaseExtensions.cs
- [x] 2. Edit Fenicia.Auth/Domains/Order/OrderController.cs to use helper instead of `Forbid(ex.Message)`
- [ ] 3. Test POST /order endpoint with invalid user-company combo (should return 403, not 500) - Manual test needed
- [ ] 4. Identify and refactor other controllers using `Forbid(ex.Message)` pattern (Fenicia.Module.Basic controllers, other Auth controllers)
- [ ] 5. Verify global ExceptionMiddleware handles remaining cases
- [x] 6. Update TODO.md after each step
