import printDocumentsList from '../documents-list';
import Message from '../message';

export default function FormOffer(form, config) {

    if (!window.dataLayer) {
        window.dataLayer = [];
    };

    // create a global array with all documents to be signed
    if (!window.documentsToBeSigned) {
        window.documentsToBeSigned = [];
    }

    window.onload = () => {
        const classes = {
            unacceptedTerms: 'unaccepted-terms',
            agreed: 'agreed',
            disabledLink: 'button-disabled',
            finishedStep: 'step--finished',
            activeStep: 'step--active',
        };
        const list = $(form.querySelector('.list-documents'));
        const listDocsToBeSigned = $(form.querySelector('.list-documents-to-be-signed'));
        const submitBtn = form.querySelector('.button-submit');
        const submitZone = form.querySelector('.submit-zone');
        const steps = [...form.querySelectorAll('.step')];

        /* Determines whether the documents have been received */
        let gotDocuments = false;
        let isDocumentsAccepted = false;

        /* Get the checkboxes dynamically */
        function getCheckboxes() {
            return Array.from(form.querySelectorAll('[type="checkbox"]'));
        }

        /* Render documents list ("documents" - from BACK-END) */
        function waitResponse() {
            /* Clear the list */
            list.children('li').remove();
        }

        waitResponse();

        /* When the documents has passed */
        function documentsReceived(documents = [],
            options = {
                agreed: false,
                checked: false,
                disabled: false,
                isRetention: false,
                isAcquisition: false,
            }) {
            gotDocuments = (documents.length > 0);
            let container = options.isRetention || options.isAcquisition ? form : list;

            $(container).removeClass('loading');

            if (gotDocuments) {
                /* Prepare the list & print the documents */
                printDocumentsList(list, listDocsToBeSigned, documents, options);

                // check if at least one document need to be signed
                // if not => delete the step
                if (steps.length && listDocsToBeSigned && !listDocsToBeSigned.children('li').length) {
                    steps[1].parentNode.removeChild(steps[1]);
                }

                // check if there is at least one document to be checked
                // if not => delete the first step and switch the icon
                if (steps.length && list && !list.children('li').length) {
                    steps[0].parentNode.removeChild(steps[0]);
                    steps.shift(); // remove the first step from array
                    const icon = $(steps[0].querySelector('.step__icon use'));
                    const newXlinkHref = icon.attr('xlink:href').replace(/#(.*)/i, '#number-one-circle');
                    icon.attr('xlink:href', newXlinkHref);
                }

                if (options.agreed) {
                    /* Mark list container as agreed (expand) */
                    list.removeClass(classes.unacceptedTerms).addClass(classes.agreed);

                    /* Hide "Check all" and submit button */
                    list.children('.check-all').remove();
                    $(submitZone).add($(submitBtn)).remove();

                    // mark all steps as finished
                    $([...steps]).addClass('step--finished');

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

                    // mark first step as active
                    if (steps.length) {
                        steps[0].classList.add(classes.activeStep);
                    }
                }

            } else {
                /* Output error on success = false */
                list.empty();
                container.removeClass(classes.unacceptedTerms).addClass('error');
                Message(list, 'appUnavailable');
            }

            return gotDocuments;
        };

        // =================================================
        //  documentsReceived(documents, { agreed: false });
        // =================================================

        /* Determine whether all checkboxes are checked + all necessary documents have been signed */
        function validateForm() {
            let validForm = true;

            const checkboxes = getCheckboxes();

            /* Determine when any of the required checkboxes are unchecked */
            checkboxes.map((checkbox) => {
                !checkbox.checked && (validForm = false);
            });

            /* Form cannot be submitted until documents are ready */
            if (!gotDocuments) {
                validForm = false;
            }

            // check if all documents have already been signed
            // or skip this step if there are no documents to be signed
            let validDocuments = true;
            if (listDocsToBeSigned.children('li').length) {
                validDocuments = validateDocuments();

                // if there are some documents to be signed (and they are already signed) =>
                // => check if offer is an acquisition => form is valid only if all checkboxes are checked
                // => otherwise make the form valid if none or all of checkboxes is checked
                const allUnchecked = areAllUnchecked();

                if (config.offerPage.isAcquisition) { // acquisition offer
                    validForm = validDocuments && validForm;
                } else { // retention offer
                    validForm = validDocuments && (allUnchecked || validForm);
                }
            }

            /* Add/Remove disabled class from the <a> button */
            if (validForm && validDocuments) {
                submitBtn.classList.remove(classes.disabledLink);
                submitBtn.removeAttribute('disabled');
            } else {
                submitBtn.classList.add(classes.disabledLink);
                submitBtn.setAttribute('disabled', 'disabled');
            }

            isDocumentsAccepted = validForm;
            return (validForm && validDocuments);
        }

        function areAllUnchecked() {
            let allUnchecked = true;
            const checkboxes = getCheckboxes();

            checkboxes.forEach(checkbox => {
                if (checkbox.checked) {
                    allUnchecked = false;
                }
            });

            return allUnchecked;
        }

        // check if all checkboxes are checked within first step
        function validateStep(step) {
            let valid = true;
            const checkboxes = [...step.querySelectorAll('[type="checkbox"]')];

            checkboxes.forEach(checkbox => {
                if (!checkbox.checked) {
                    valid = false;
                }
            });

            if (valid) {
                step.classList.add(classes.finishedStep);
            } else {
                step.classList.remove(classes.finishedStep);
            }

            return valid;
        }

        function validateDocuments() {
            let allSigned = true;

            if (documentsToBeSigned.length) {
                documentsToBeSigned.forEach(document => {
                    !document.signed && (allSigned = false);
                });
            } else {
                allSigned = false;
            }

            return allSigned;
        }

        /* Handle form change */
        form.onchange = () => {
            if (steps.length) {
                if (validateStep(steps[0])) {
                    steps[1].classList.add(classes.activeStep);
                } else {
                    steps[1].classList.remove(classes.activeStep);
                }
            }

            validateForm();
        };

        // if some document have been signed => revalidate form
        $(form).on('retention.document.signed', () => {
            const allSigned = validateDocuments();

            if (allSigned && steps.length) {
                if (steps.length === 1) {
                    steps[0].classList.add(classes.finishedStep);
                } else {
                    steps[1].classList.add(classes.finishedStep);
                }
            }

            validateForm();
        });

        /* Validate on window load for History back */
        validateForm();

        /* Handle submit and validate */
        submitBtn.onclick = (event) => {
            if (!isDocumentsAccepted) {
                event.preventDefault();
            } else {
                dataLayer.push({
                    event: 'gaEvent',
                    gaEventData: {
                        eCat: 'eContracting',
                        eAct: 'Offer accepted'
                    },
                    eventCallback: function() {
                        dataLayer.push({ gaEventData: undefined });
                    }
                });
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
                timeout: 10000,
                error: function(xhr, textStatus) {
                    if (textStatus === 'timeout') {
                        let container =
                          config.offerPage &&
                          (config.offerPage.isRetention ||
                            config.offerPage.isAcquisition)
                            ? form
                            : list;
                        list.empty();
                        $(container).removeClass('loading').addClass('error');
                        Message(list, 'appUnavailable');
                    } else {
                        window.location.href = '/404';
                    }
                },
                success: function(documents) {
                    var agreed = config.offerPage.isAgreed;
                    var isRetention = config.offerPage.isRetention;
                    var isAcquisition = config.offerPage.isAcquisition;
                    documentsReceived(documents, {
                        agreed: agreed,
                        isRetention: isRetention,
                        isAcquisition: isAcquisition
                    });
                }
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
