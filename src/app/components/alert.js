export default function Alert(message, type = 'warning', container = null) {

    /* HTML template */
    const template = [
        `<div class="alert ${type}">`,
        `${message}<a class="close" href=""></a>`,
        '</div>'
    ].join('');

    /* When container argument is set, append to container */
    if (container) {
        container = container instanceof jQuery && $(container);
        container.append(template);
        // container.children('.alert').hide();
    }

    /* Handle alert close */
    $('.alert .close').on('click', (e) => {
        const link = $(e.target);
        e.preventDefault();

        const alertBox = $(e.target).parent('.alert');
        alertBox.slideUp(250).fadeOut(0);
    });

    return template;
}