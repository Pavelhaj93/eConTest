import printDocumentsList from '../documents-list';
import Message from '../message';

export default function FormOffer(form, config) {

    window.onload = () => {
        const classes = {
            unacceptedTerms: 'unaccepted-terms',
            agreed: 'agreed',
            disabledLink: 'button-disabled'
        };
        const list = $(form.querySelector('.list'));
        const submitBtn = form.querySelector('.button-submit');

        /* Determines whether the documents have been received */
        let gotDocuments = false;
        let isDocumentsAccepted = false;

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
        function documentsReceived(documents = [],
            options = {
                agreed: false,
                checked: false,
                disabled: false
            }) {
            gotDocuments = (documents.length > 0);
            list.removeClass('loading');

            if (gotDocuments) {
                /* Prepare the list & print the documents */
                printDocumentsList(list, documents, options);

                if (options.agreed) {
                    /* Mark list container as agreed (expand) */
                    list.removeClass(classes.unacceptedTerms).addClass(classes.agreed);

                    /* Hide "Check all" and submit button */
                    list.children('.check-all').remove();
                    $(submitBtn).remove();

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
        //  documentsReceived(documents, { agreed: false });
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
            if (!gotDocuments) {
                valid = false;
            }

            /* Add/Remove disabled class from the <a> button */
            if (valid) {
                submitBtn.classList.remove(classes.disabledLink);
            } else {
                submitBtn.classList.add(classes.disabledLink);
            }

            isDocumentsAccepted = valid;
            return valid;
        }

        /* Handle form change */
        form.onchange = () => {
            validateForm();
        };

        /* Validate on window load for History back */
        validateForm();

        /* Handle submit and validate */
        submitBtn.onclick = (event) => {
            if (!isDocumentsAccepted) {
                event.preventDefault();
            }
        };

        /* Moved from DocumentPanel */
        // ---------------------------

        // var offerPageOptions = {
        //         doxReadyUrl: '@Url.Content("~/Areas/eContracting/Services/DoxReady.ashx?id")=@Model.ClientId',
        //         isAgreed: @Model.IsAccepted.ToString().ToLower(),
        //         getFileUrl: '@Url.Content("~/Areas/eContracting/Services/GetFile.ashx?file=")'
        //     }

        function CheckIfReady() {
            $.ajax({
                type: 'POST',
                url: config.offerPage.doxReadyUrl,
                dataType: 'json',
                timeout: 30000,
                error: function() {
                    window.location.href = '/404';
                },
                success: function(documents) {
                    var agreed = config.offerPage.isAgreed;
                    documentsReceived(documents, { agreed: agreed });
                },
            });
        }

        if (config.offerPage) {
            CheckIfReady();
        }

        window.handleClick = function(e, key) {
            e.preventDefault();
            window.location.href = config.offerPage.getFileUrl + key;
        };

    };
}
