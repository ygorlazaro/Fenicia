using Fenicia.Module.Basic.Domains.ProductCategory.Update;
using FluentValidation.TestHelper;

namespace Fenicia.Module.Basic.Tests.Domains.ProductCategory;

public class UpdateProductCategoryValidatorTests
{
    private readonly UpdateProductCategoryValidator validator = new();

    [Fact]
    public void Validate_WhenIdAndNameAreValid_ShouldNotHaveErrors()
    {
        // Arrange
        var command = new UpdateProductCategoryCommand(Guid.NewGuid(), "Valid Category Name");

        // Act
        var result = this.validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenIdIsEmpty_ShouldHaveError()
    {
        // Arrange
        var command = new UpdateProductCategoryCommand(Guid.Empty, "Valid Category Name");

        // Act
        var result = this.validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Id);
    }

    [Fact]
    public void Validate_WhenNameIsEmpty_ShouldHaveError()
    {
        // Arrange
        var command = new UpdateProductCategoryCommand(Guid.NewGuid(), string.Empty);

        // Act
        var result = this.validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Name);
    }

    [Fact]
    public void Validate_WhenNameIsNull_ShouldHaveError()
    {
        // Arrange
        var command = new UpdateProductCategoryCommand(Guid.NewGuid(), null!);

        // Act
        var result = this.validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Name);
    }

    [Fact]
    public void Validate_WhenBothIdAndNameAreInvalid_ShouldHaveBothErrors()
    {
        // Arrange
        var command = new UpdateProductCategoryCommand(Guid.Empty, string.Empty);

        // Act
        var result = this.validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Id);
        result.ShouldHaveValidationErrorFor(c => c.Name);
    }

    [Fact]
    public void Validate_WhenNameHasValidContent_ShouldNotHaveError()
    {
        // Arrange
        var command = new UpdateProductCategoryCommand(Guid.NewGuid(), "Electronics");

        // Act
        var result = this.validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(c => c.Name);
    }

    [Fact]
    public void Validate_WhenIdIsNewGuid_ShouldNotHaveError()
    {
        // Arrange
        var command = new UpdateProductCategoryCommand(Guid.NewGuid(), "Category");

        // Act
        var result = this.validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(c => c.Id);
    }

    [Fact]
    public void Validate_WhenNameContainsSpecialCharacters_ShouldNotHaveError()
    {
        // Arrange
        var command = new UpdateProductCategoryCommand(Guid.NewGuid(), "Category & Sons - Ltd.");

        // Act
        var result = this.validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(c => c.Name);
    }

    [Fact]
    public void Validate_WhenNameContainsNumbers_ShouldNotHaveError()
    {
        // Arrange
        var command = new UpdateProductCategoryCommand(Guid.NewGuid(), "Category 2024");

        // Act
        var result = this.validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(c => c.Name);
    }

    [Fact]
    public void Validate_WhenNameHasOnlyWhitespace_ShouldHaveError()
    {
        // Arrange
        var command = new UpdateProductCategoryCommand(Guid.NewGuid(), "   ");

        // Act
        var result = this.validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Name);
    }
}
