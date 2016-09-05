export default function printDocumentsList(container, documents) {
    !(container instanceof jQuery) && (container = $(container));

    /* Loop through each document */
    documents.map((doc, i) => {
        const key = i + 1;
        const title = doc.title;
        const url = doc.url;
        const item = [
            '<li>',
            `<input id="document-${key}" type="checkbox" autocomplete="off">`,
            `<label for="document-${key}">`,
            `Souhlasím s <a class="pdf" href="${url}" title="${title}" target="_blank">${title}</a>`,
            '</label>',
            '</li>'
        ].join('');

        /* Append HTML */
        container.append(item);

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
