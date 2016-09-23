import printDocumentsList from '../documents-list';
import Message from '../message';

export default function FormOffer(form) {

    window.onload = () => {
        const classUnagreed = 'unaccepted-first';
        const list = $(form.querySelector('.list'));
        const submitBtn = form.querySelector('[type="submit"]');

        /* Determines whether the documents have been received */
        let gotDocuments = false;

        /* Get the checkboxes dynamically */
        function getCheckboxes() {
            return Array.from(form.querySelectorAll('[type="checkbox"]'));
        }

        /* Render documents list ("documents" - from BACK-END) */
        function waitResponse() {

            /* Reset customerAgreement */
            list.addClass(classUnagreed);

            /* Clear the list */
            list.children('li').remove();
        }
        waitResponse();

        /* When the documents has passed */
        window.documentsReceived = function(documents = [], options = { checked: false, disabled: false}) {
            gotDocuments = (documents.length > 0) && true;
            list.removeClass('loading');

            if (gotDocuments) {
                /* Establish, that the documents are ready */
                gotDocuments = true;

                /* Prepare the list & print the documents */
                printDocumentsList(list, documents, options);

                /* Show other list items by clicking on customerAgreement */
                const customerAgreement = list.children('li:first-child').children('input[type="checkbox"]');

                /* Handle customerAgreement click */
                customerAgreement.on('change', () => {
                    const agreed = !list.hasClass(classUnagreed);
                    const onlyChild = (list.children('li').length == 1);

                    if (!agreed && !onlyChild) {
                        /* Reveal the documents */
                        list.removeClass(classUnagreed);
                    }
                });
            } else {
                /* Output error on success = false */
                list.empty();
                list.removeClass(classUnagreed).addClass('error');
                Message(list, 'appUnavailable');
            }

            return gotDocuments;
        };
        // documentsReceived([{s: 2}]);

        /* Determine whether all checkboxes are checked */
        function validateForm() {
            let valid = true;
            const checkboxes = getCheckboxes();

            /* Determine when any of the required checkboxes are unchecked */
            checkboxes.map((checkbox) => {
                !checkbox.checked && (valid = false);
            });

            /* Form cannot be submitted until documents are ready */
            if (!gotDocuments) { valid = false; }

            submitBtn.disabled = !valid;
            return valid;
        }

        /* Handle form change */
        form.onchange = () => {
            validateForm();
        };

        /* Handle form submit */
        submitBtn.onclick = () => {

            /* Safety for manual removal of "disabled" attribute */
            if (gotDocuments) {
                window.location = 'thank-you.html';
            }
        };

        /* Validate on window load for History back */
        validateForm();
    };
}
