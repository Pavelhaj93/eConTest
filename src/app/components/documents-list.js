export default function printDocumentsList(container, documents, options) {
    !(container instanceof jQuery) && (container = $(container));

    /* Custom options */
    const checked = options.checked || false;
    const disabled = options.disabled || false;

    console.log(options.agreed, checked, disabled);

    /* DEMO: Method is overwritten on server */
    // window.handleClick = function(e, key) {
    //     e.preventDefault();
    // };
    /* END DEMO */

    /* Loop through each document */
    documents.map((doc, i) => {
        const key = i + 1;
        const label = doc.label;
        const title = doc.title;
        const value = doc.id;
        const item = [
            '<li>',
            !options.agreed && `
            <input
                id="document-${key}"
                type="checkbox"
                name="documents"
                value="${value}"
                ${checked && 'checked'}
                ${disabled && 'disabled'}
                autocomplete="off">`,
            `<label for="document-${key}">`,
            !options.agreed && `${label} `,
            `<a class="pdf" href="#" data-key="${key}" title="${title}">${title}</a>`,
            '</label>',
            '</li>'
        ].filter(Boolean).join('');

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
};
