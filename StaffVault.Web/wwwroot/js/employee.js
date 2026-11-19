// StaffVault - page behaviour and client-side validation.
// The server checks every rule again; this file only gives instant feedback.
document.addEventListener("DOMContentLoaded", function () {
    const MAX_PICTURE_BYTES = 2 * 1024 * 1024; // 2 MB
    const ALLOWED_EXTENSIONS = [".jpg", ".jpeg", ".png"];
    const EMAIL_PATTERN = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

    // ================= 1. Toast message =================
    const toastElement = document.getElementById("messageToast");
    if (toastElement) {
        new bootstrap.Toast(toastElement, { delay: 3000 }).show();
    }

    // ================= 2. Delete modal =================
    const deleteModal = document.getElementById("deleteModal");
    if (deleteModal) {
        deleteModal.addEventListener("show.bs.modal", function (event) {
            const button = event.relatedTarget; // the Delete button that was clicked

            document.getElementById("deleteEmployeeName").textContent = button.dataset.name;
            document.getElementById("deleteEmployeeId").value = button.dataset.id;
        });
    }

    // ================= 3. Form validation =================
    const form = document.getElementById("employeeForm");
    if (!form) {
        return;
    }

    const nameInput = document.getElementById("Name");
    const dobInput = document.getElementById("DateOfBirth");
    const emailInput = document.getElementById("Email");
    const pictureInput = document.getElementById("Picture");

    // The calendar cannot pick today or a future date.
    const today = toLocalDateString(new Date());
    const yesterday = new Date();
    yesterday.setDate(yesterday.getDate() - 1);
    dobInput.max = toLocalDateString(yesterday);

    form.addEventListener("submit", function (event) {
        // Run every check so all errors show at once.
        const results = [validateName(), validateDateOfBirth(), validateEmail(), validatePicture()];

        if (results.includes(false)) {
            event.preventDefault(); // stop the form from posting
        }
    });

    function validateName() {
        if (nameInput.value.trim() === "") {
            return setError("Name", "Name is required.");
        }
        return setError("Name", "");
    }

    // Rule: Date of Birth must be less than today.
    function validateDateOfBirth() {
        if (dobInput.value === "") {
            return setError("DateOfBirth", "Date of Birth is required.");
        }
        if (dobInput.value >= today) { // both "yyyy-MM-dd", so text comparison works as date comparison
            return setError("DateOfBirth", "Date of Birth must be earlier than today.");
        }
        return setError("DateOfBirth", "");
    }

    // Rule: basic email validation.
    function validateEmail() {
        const email = emailInput.value.trim();
        if (email === "") {
            return setError("Email", "Email is required.");
        }
        if (!EMAIL_PATTERN.test(email)) {
            return setError("Email", "Please enter a valid email address.");
        }
        return setError("Email", "");
    }

    // Rule: only .jpg, .jpeg, .png (and max 2 MB). Picture is optional.
    function validatePicture() {
        if (pictureInput.files.length === 0) {
            return setError("Picture", "");
        }

        const file = pictureInput.files[0];
        const fileName = file.name.toLowerCase();
        const hasAllowedExtension = ALLOWED_EXTENSIONS.some(ext => fileName.endsWith(ext));

        if (!hasAllowedExtension) {
            return setError("Picture", "Only .jpg, .jpeg and .png files are allowed.");
        }
        if (file.size > MAX_PICTURE_BYTES) {
            return setError("Picture", "Picture must be 2 MB or smaller.");
        }
        return setError("Picture", "");
    }

    // ================= Helpers =================

    // Writes the message into the <span asp-validation-for="..."> of the field.
    // Returns true when the message is empty (field valid), false otherwise.
    function setError(fieldName, message) {
        const span = document.querySelector(`[data-valmsg-for="${fieldName}"]`);
        if (span) {
            span.textContent = message;
        }
        return message === "";
    }

    // Local date as "yyyy-MM-dd" (toISOString uses UTC, which is wrong in India/UAE after midnight).
    function toLocalDateString(date) {
        const month = String(date.getMonth() + 1).padStart(2, "0");
        const day = String(date.getDate()).padStart(2, "0");
        return `${date.getFullYear()}-${month}-${day}`;
    }
});