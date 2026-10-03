// Submit handler for the form in index.html
function submitForm(event) {
  event.preventDefault();

  const form = document.querySelector('form');
  if (!form) {
    console.warn('No form found in index.html');
    return;
  }

  const submitButton = document.querySelector('button[type="submit"]');
  if (submitButton) {
    submitButton.disabled = true;
    submitButton.textContent = 'Submitting...';
  }

  // Gather form data
  const formData = new FormData(form);

  // Example: validate required fields
  const name = formData.get('name')?.toString().trim();
  if (!name) {
    alert('Please enter your name.');
    if (submitButton) {
      submitButton.disabled = false;
      submitButton.textContent = 'Submit';
    }
    return;
  }

  // Replace this with your actual submit logic
  console.log('Form submitted:', Object.fromEntries(formData.entries()));
  alert('Form submitted successfully!');

  form.reset();

  if (submitButton) {
    submitButton.disabled = false;
    submitButton.textContent = 'Submit';
  }
}

document.addEventListener('DOMContentLoaded', () => {
  const submitButton = document.querySelector('button[type="submit"]');
  if (submitButton) {
    submitButton.addEventListener('click', submitForm);
  }
});
