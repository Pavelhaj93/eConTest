export default function CheckAll(container) {
    const form = container.closest('form');

    /* Handle click */
    container.onclick = (e) => {
        e.preventDefault();
        const link = e.target;

        /* Get checkboxes on click due to their dynamic nature */
        const checkboxes = Array.from(form.querySelectorAll('input[type="checkbox"]'));

        /* Check all checkboxes */
        checkboxes.map((checkbox) => {
            checkbox.checked = !link.classList.contains('checked');
        });
        
        /* Toggle the checked state */
        link.classList.toggle('checked');

        /* Update the form's state */
        form.onchange();
    };

}