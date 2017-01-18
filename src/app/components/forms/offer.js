import printDocumentsList from '../documents-list';
import Message from '../message';

export default function FormOffer(form) {

    window.onload = () => {
        const classes = {
            unacceptedTerms: 'unaccepted-terms',
            agreed: 'agreed',
            disabledLink: 'button-disabled'
        };
        const list = $(form.querySelector('.list'));
        const submitBtn = form.querySelector('.button-submit');

        console.log('form', form);
        console.log('submitBtn on init:', submitBtn);

        /* Determines whether the documents have been received */
        let gotDocuments = false;

        /* Get the checkboxes dynamically */
        function getCheckboxes() {
            return Array.from(form.querySelectorAll('[type="checkbox"]'));
        }

        /* Render documents list ("documents" - from BACK-END) */
        function waitResponse() {
            /* Reset customerAgreement */
            list.addClass(classes.unacceptedTerms);

            /* Clear the list */
            list.children('li').remove();
        }
        waitResponse();

        /* When the documents has passed */
        window.documentsReceived = function(documents = [],
            options = {
                agreed: false,
                checked: false,
                disabled: false
            }) {
            gotDocuments = (documents.length > 0);
            list.removeClass('loading');

            console.warn('documentsReceived');
            console.log(documents, options);

            if (gotDocuments) {
                console.log('got the documents successfully');

                /* Prepare the list & print the documents */
                printDocumentsList(list, documents, options);

                if (options.agreed) {
                    console.log('agreed to the documents, printing accepted-offer');
                    /* Mark list container as agreed (expand) */
                    list.removeClass(classes.unacceptedTerms).addClass(classes.agreed);

                    /* Hide "Check all" and submit button */
                    list.children('.check-all').remove();

                    console.log('removed all .check-all children');
                    console.log('submitBtn', submitBtn);

                    submitBtn.remove();

                    console.log('submit btn should be removed');

                } else {
                    /* Show other list items by clicking on customerAgreement */
                    const customerAgreement = list.children('li:first-child').children('input[type="checkbox"]');

                    /* Handle customerAgreement click */
                    customerAgreement.on('change', () => {
                        const agreed = !list.hasClass(classes.unacceptedTerms);
                        const onlyChild = (list.children('li').length == 1);

                        if (!agreed && !onlyChild) {
                            /* Reveal the documents */
                            list.removeClass(classes.unacceptedTerms);
                        }
                    });
                }

            } else {
                /* Output error on success = false */
                list.empty();
                list.removeClass(classes.unacceptedTerms).addClass('error');
                Message(list, 'appUnavailable');
            }

            return gotDocuments;
        };

        // =================================================
        // documentsReceived(documents, { agreed: false });
        // =================================================

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

            /* Add/Remove disabled class from the <a> button */
            if (valid) {
                submitBtn.classList.remove(classes.disabledLink);
            } else {
                submitBtn.classList.add(classes.disabledLink);
            }

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
