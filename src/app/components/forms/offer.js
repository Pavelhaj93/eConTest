import printDocumentsList from '../documents-list';
import Message from '../message';

export default function FormOffer(form) {

    window.onload = () => {
        let receivedDocuments = false;
        const list = $(form.querySelector('.list'));
        const additional = document.getElementById('customer-rights');
        const submitBtn = form.querySelector('[type="submit"]');

        /* Get the checkboxes dynamically */
        function getCheckboxes() {
            return Array.from(form.querySelectorAll('input[type="checkbox"]'));
        }

        /* Render documents list ("documents" - from BACK-END) */
        function renderDocuments() {

            /* Clear the list */
            list.children('li').remove();

            // ======================================================
            // REMOVE: presentation
            // printDocumentsList(list, documents);
            // receivedDocuments = true;
            // END REMOVE
            // ======================================================

            /* Request the documents */
            $.ajax({
                type: 'GET',
                dataType: 'jsonp',
                url: 'http://google.com',

                beforeSend: () => {
                    /* Display loading */
                    list.addClass('loading');

                    /* One second timeout to determine if server is down */
                    setTimeout(() => {
                        // WIP
                    }, 1000);
                },

                success: (response) => {
                    const documents = response.documents;
                    receivedDocuments = documents.length > 0;

                    receivedDocuments && printDocumentsList(list, documents);
                },

                /* UNCOMMENT: For normal behavior */
                error: () => {
                    list.addClass('error');
                    Message(list, 'appUnavailable');
                },

                complete: () => {
                    list.removeClass('loading');
                }
            });
        }
        renderDocuments();

        /* Determine whether all checkboxes are checked */
        function validate() {
            let valid = true;
            const checkboxes = getCheckboxes();

            /* Determine when any of the required checkboxes are unchecked */
            checkboxes.map((checkbox) => {
                !checkbox.checked && (valid = false);
            });

            /* When additional is not checked OR response not received, could not validate */
            if (!additional.checked || !receivedDocuments) {
                valid = false;
            }

            submitBtn.disabled = !valid;
            return valid;
        }

        /* Handle additional checkbox click */
        additional.onchange = () => {
            form.onchange();

            /* Show the documents list if not visible */
            if (!list.is(':visible')) {
                $(list).slideDown(300);
            }
        };

        /* Handle form change */
        form.onchange = (e) => {
            validate();
        };

        /* Handle form submit */
        form.onsubmit = (e) => {
            e.preventDefault();

            /* Safety for manual removal of "disabled" attribute */
            if (receivedDocuments) {

                // ======================================================
                // REMOVE: example
                window.location = 'thank-you.html';
                // ======================================================
            }
        };

        /* Validate on window load for History back */
        validate();
    };
}
