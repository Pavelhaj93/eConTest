export default function printDocumentsList(container, docsToBeSignedContainer, documents, options) {
    !(container instanceof jQuery) && (container = $(container));
    !(docsToBeSignedContainer instanceof jQuery) && (docsToBeSignedContainer = $(docsToBeSignedContainer));

    /* Custom options */
    const checked = options.checked || false;
    const disabled = options.disabled || false;

    // console.log(options.agreed, checked, disabled);

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
            (!options.agreed && !doc.sign) && `
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
        if (options.isRetention || options.isAcquisition) { // check if offer is retention or acquisition
            if (doc.sign) {
                docsToBeSignedContainer.append(item); // documents to be signed
                window.documentsToBeSigned.push({
                    key: key,
                    signed: options.agreed,
                });
            } else {
                container.append(item); // other documents
            }
        } else { // default offer
            if (i == 0) {
                container.prepend(item);
            } else {
                container.append(item);
            }
        }

        // specify where to find newly added item
        const list = doc.sign ? docsToBeSignedContainer : container;
        const link = list.children('li:last-of-type').find('a');

        /* Attach click event */
        link.on('click', (e) => {
            handleClick(e, key);
        });
    });
};
