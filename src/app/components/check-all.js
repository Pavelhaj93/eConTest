export default function CheckAll(container) {
    container = $(container);
    const form = container.closest('.form');

    /* Handle click */
    container.on('click', (e) => {
        e.preventDefault();
        const link = e.target;

        /* Get checkboxes on click due to their dynamic nature */
        const checkboxes = form.find('input[type="checkbox"]');

        /* Check all checkboxes */
        checkboxes.map((i, checkbox) => {
            checkbox.checked = !link.classList.contains('checked');
        });

        /* Toggle the checked state */
        link.classList.toggle('checked');

        /* Update the form's state */
        form.change();
    });

}
