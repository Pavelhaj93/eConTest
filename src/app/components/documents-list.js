export default function printDocumentsList(container, documents) {
    !(container instanceof jQuery) && (container = $(container));

    /* DEMO: Method is overwritten on server */
    // window.handleClick = function(e, key) {
    //     e.preventDefault();
    // };
    /* END DEMO */

    /* Loop through each document */
    documents.map((doc, i) => {
        const key = i + 1;
        const title = doc.title;
        const item = [
            '<li>',
            `<input id="document-${key}" type="checkbox" autocomplete="off">`,
            `<label for="document-${key}">`,
            `Souhlasím s <a class="pdf" href="#" data-key="${key}" title="${title}">${title}</a>`,
            '</label>',
            '</li>'
        ].join('');

        /* Insert HTML */
        if (i == 0) {
            container.prepend(item);
        } else {
            container.append(item);
        }

        /* Attach click event */
        const link = container.children('li:last-of-type').find('a');
        link.on('click', (e) => {
            handleClick(e, key);
        });
    });

    /* Add custom properties */
    const options = arguments[2];
    if (options) {
        const checked = options.checked || false;
        const disabled = options.disabled || false;
        let input = container.find('input');

        /* Checked */
        input.prop('checked', checked);

        /* Disabled */
        disabled && input.prop('disabled', true);
    }
};
