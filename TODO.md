# Unit Test Fixes for Failing StockMovement Tests

**Status**: LEFT JOIN fix applied, but still 10 fails - root cause orphan Product.CategoryId (Guid.NewGuid(), no CategoryModel added). EF projection m.Product.Name or Category.Name fails or filters out.

## Updated Plan
### [x] 1. Create TODO.md (DONE)
### [x] 2. Fix GetStockMovementHandler.cs LEFT JOINs (DONE)
### [x] 3. Fix GetStockMovementDashboardHandler.cs LEFT JOINs (DONE)
### [ ] 4. Add CategoryModel before ProductModel in failing tests (3 files):
   - GetStockMovementHandlerTests.cs: Handle_WithPagination_ReturnsCorrectPage, Handle_WithMovementsInDateRange_ReturnsFilteredList
   - GetStockMovementDashboardHandlerTests.cs: Handle_WithMovements_ReturnsStockMovementHistory, Handle_WithCustomDaysFilter..., Handle_WithDateRangeFilter..., Handle_WithCustomerMovement..., Handle_WithSupplierMovement..., Handle_WithMultipleMovements_ReturnsHistoryOrderedByDateDescending
   - StockMovementControllerTests.cs: GetAsync_WhenMovementsExist_ReturnsOkWithMovements, GetDashboardAsync_WithMovements_ReturnsDashboardData
### [ ] 5. Re-run `dotnet test Fenicia.sln --logger "console;verbosity=detailed"` to verify 0 fails
### [ ] 6. Mark COMPLETE, remove TODO.md, attempt_completion

**Expected**: All tests pass after adding CategoryModel var category = new ProductCategoryModel { Id = product.CategoryId, Name = "Test Category" }; db.BasicProductCategories.Add(category); before SaveChanges.
