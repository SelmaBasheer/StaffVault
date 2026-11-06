using Microsoft.AspNetCore.Mvc;
using StaffVault.Application.Employees;
using StaffVault.Web.Models;

namespace StaffVault.Web.Controllers;

public sealed class EmployeeController(IEmployeeService employeeService) : Controller
{
    // GET /Employee            -> empty form + grid
    // GET /Employee?editId=5   -> form filled with employee 5 + grid
    public async Task<IActionResult> Index(int? editId)
    {
        var model = new EmployeeViewModel();

        if (editId > 0)
        {
            var employee = await employeeService.GetByIdAsync(editId.Value);
            if (employee is not null)
            {
                model.Id = employee.Id;
                model.Name = employee.Name;
                model.DateOfBirth = employee.DateOfBirth;
                model.Email = employee.Email;
                model.ExistingPicturePath = employee.PicturePath;
            }
        }

        model.Employees = await employeeService.GetAllAsync();
        return View(model);
    }

    // POST /Employee/Save -> Add or Update
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(EmployeeViewModel model)
    {
        if (ModelState.IsValid)
        {
            await using var stream = model.Picture?.OpenReadStream();
            var picture = model.Picture is null
                ? null
                : new FileUpload(stream!, model.Picture.FileName, model.Picture.Length);

            var request = new SaveEmployeeRequest(
                model.Name,
                model.DateOfBirth!.Value,
                model.Email,
                picture
            );

            var result =
                model.Id == 0
                    ? await employeeService.CreateAsync(request)
                    : await employeeService.UpdateAsync(model.Id, request);

            if (result.IsSuccess)
            {
                TempData["Message"] = model.Id == 0 ? "Employee added." : "Employee updated.";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Field, error.Message);
            }
        }

        // Something failed: show the page again with the errors.
        model.Employees = await employeeService.GetAllAsync();
        return View(nameof(Index), model);
    }

    // POST /Employee/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await employeeService.DeleteAsync(id);
        TempData["Message"] = "Employee deleted.";
        return RedirectToAction(nameof(Index));
    }

    [Route("error")]
    public IActionResult Index() => View("Error");
}
