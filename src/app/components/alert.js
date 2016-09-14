/*
*
* Alert
*
* ARGUMENTS:
* message (String) - The message to output in the alert box.
* type (String) - The class of the alert box.
* container (DOMElement/jQuery) [Optional] - Container to append the message to.
*
*/

export default function Alert(message, type = 'warning', container = null) {
    /* Convert Array messages to String */
    (message instanceof Array) && (message = message.join(''));

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
    }

    /* Handle alert close */
    $('.alert .close').on('click', (e) => {
        e.preventDefault();

        const link = $(e.target);
        const alertBox = $(e.target).parent('.alert');

        alertBox.addClass('closing').slideUp(250).fadeOut(0, () => {
            alertBox.remove();
        });
    });

    return template;
}