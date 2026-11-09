using Microsoft.AspNetCore.Mvc;
using StaffVault.Application.Abstractions;
using StaffVault.Application.Common;
using StaffVault.Application.Employees;
using StaffVault.Web.Models;

namespace StaffVault.Web.Controllers;

public sealed class EmployeeController(IEmployeeService employeeService) : Controller
{
    // GET /Employee  -> empty form + grid
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var model = new EmployeeViewModel();
        return await ShowPageAsync(model, cancellationToken);
    }

    // GET /Employee/Edit/5  -> form filled with employee 5 + grid
    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var employee = await employeeService.GetByIdAsync(id, cancellationToken);
        if (employee is null)
        {
            TempData["ErrorMessage"] = "Employee not found.";
            return RedirectToAction(nameof(Index));
        }

        var model = new EmployeeViewModel
        {
            Id = employee.Id,
            Name = employee.Name,
            DateOfBirth = employee.DateOfBirth.ToDateTime(TimeOnly.MinValue),
            Email = employee.Email,
            ExistingPicturePath = employee.PicturePath,
        };

        return await ShowPageAsync(model, cancellationToken);
    }

    // POST /Employee/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        EmployeeViewModel model,
        CancellationToken cancellationToken
    )
    {
        if (!ModelState.IsValid)
        {
            return await ShowPageAsync(model, cancellationToken);
        }

        await using var pictureStream = model.Picture?.OpenReadStream();
        var request = new CreateEmployeeRequest(
            model.Name,
            DateOnly.FromDateTime(model.DateOfBirth!.Value),
            model.Email,
            ToFileUpload(model.Picture, pictureStream)
        );

        var result = await employeeService.CreateAsync(request, cancellationToken);
        return await HandleResultAsync(
            result,
            model,
            "Employee added successfully.",
            cancellationToken
        );
    }

    // POST /Employee/Update
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(
        EmployeeViewModel model,
        CancellationToken cancellationToken
    )
    {
        if (!ModelState.IsValid)
        {
            return await ShowPageAsync(model, cancellationToken);
        }

        await using var pictureStream = model.Picture?.OpenReadStream();
        var request = new UpdateEmployeeRequest(
            model.Id,
            model.Name,
            DateOnly.FromDateTime(model.DateOfBirth!.Value),
            model.Email,
            ToFileUpload(model.Picture, pictureStream)
        );

        var result = await employeeService.UpdateAsync(request, cancellationToken);
        return await HandleResultAsync(
            result,
            model,
            "Employee updated successfully.",
            cancellationToken
        );
    }

    // POST /Employee/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await employeeService.DeleteAsync(id, cancellationToken);

        if (result.IsSuccess)
        {
            TempData["SuccessMessage"] = "Employee deleted successfully.";
        }
        else
        {
            TempData["ErrorMessage"] = result.Errors[0].Message;
        }

        return RedirectToAction(nameof(Index));
    }

    // ---------------- Helpers ----------------

    // Loads the grid and shows the single Index view.
    private async Task<IActionResult> ShowPageAsync(
        EmployeeViewModel model,
        CancellationToken cancellationToken
    )
    {
        model.Employees = await employeeService.GetAllAsync(cancellationToken);
        return View(nameof(Index), model);
    }

    // Success -> message + redirect. Failure -> errors under the inputs.
    private async Task<IActionResult> HandleResultAsync(
        Result result,
        EmployeeViewModel model,
        string successMessage,
        CancellationToken cancellationToken
    )
    {
        if (result.IsSuccess)
        {
            TempData["SuccessMessage"] = successMessage;
            return RedirectToAction(nameof(Index));
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(error.Field, error.Message);
        }

        return await ShowPageAsync(model, cancellationToken);
    }

    // IFormFile (ASP.NET type) -> FileUpload (plain type the Application layer understands)
    private static FileUpload? ToFileUpload(IFormFile? file, Stream? stream) =>
        file is null || stream is null ? null : new FileUpload(stream, file.FileName, file.Length);
}
