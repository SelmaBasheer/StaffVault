// StaffVault - client-side validations (the server checks everything again).
document.addEventListener("DOMContentLoaded", function () {
    const form = document.getElementById("employeeForm");
    const dob = document.getElementById("DateOfBirth");

    // Don't allow picking today or a future date in the calendar.
    const today = new Date().toISOString().split("T")[0]; // "yyyy-MM-dd"
    dob.max = today;

    form.addEventListener("submit", function (event) {
        let isValid = true;

        // 1. Name required
        if (document.getElementById("Name").value.trim() === "") {
            showError("Name", "Name is required.");
            isValid = false;
        } else {
            showError("Name", "");
        }

        // 2. Date of Birth must be less than today
        if (dob.value === "") {
            showError("DateOfBirth", "Date of Birth is required.");
            isValid = false;
        } else if (dob.value >= today) {
            showError("DateOfBirth", "Date of Birth must be earlier than today.");
            isValid = false;
        } else {
            showError("DateOfBirth", "");
        }

        // 3. Basic email validation
        const email = document.getElementById("Email").value.trim();
        const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!emailPattern.test(email)) {
            showError("Email", "Please enter a valid email address.");
            isValid = false;
        } else {
            showError("Email", "");
        }

        // 4. Picture: only .jpg, .jpeg, .png
        const picture = document.getElementById("Picture");
        if (picture.files.length > 0) {
            const fileName = picture.files[0].name.toLowerCase();
            const allowed = fileName.endsWith(".jpg") || fileName.endsWith(".jpeg") || fileName.endsWith(".png");
            if (!allowed) {
                showError("Picture", "Only .jpg, .jpeg and .png files are allowed.");
                isValid = false;
            } else {
                showError("Picture", "");
            }
        }

        if (!isValid) {
            event.preventDefault(); // stop the form from posting
        }
    });

    // Puts the message into the <span asp-validation-for="..."> of that field.
    function showError(fieldName, message) {
        const span = document.querySelector(`[data-valmsg-for="${fieldName}"]`);
        if (span) {
            span.textContent = message;
        }
    }
});