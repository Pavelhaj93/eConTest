import Message from '../message';

export default function SignatureModal(el, config) {
    (function($) {
        const target = $(el).data('target');
        const $modal = $(target);
        const $signature = $modal.find('.js-signature');
        const $clearBtn = $modal.find('.js-signature-clear-btn');
        const $saveBtn = $modal.find('.js-signature-save-btn');
        let errorMesssageVisible = false;

        // open modal handler
        el.addEventListener('click', function(event) {
            event.preventDefault();

            if ($modal.length) {
                $modal.modal('show');
            } else {
                console.error(`Modal element ${target} not found on the page.`);
            }
        });

        // clear handler
        $clearBtn.click(function(event) {
            event.preventDefault();

            if ($.fn.jSignature) {
                $signature.jSignature('clear'); // clear the signature pad
            }
        });

        // save handler
        $saveBtn.click(function(event) {
            event.preventDefault();

            // check if user "draw" something in canvas
            if ($.fn.jSignature && $signature.jSignature('isModified')) {
                $modal.find('.modal-content').addClass('loading');

                // get PNG as base 64
                const signatureData = $signature.jSignature('getData', 'image')[1];

                // id of document that is going to signed
                const document = window.documentsToBeSigned[0];

                $.ajax({
                    type: 'POST',
                    url: config.offerPage.signFileUrl + document.key,
                    data: {
                        signature: signatureData
                    }
                }).done(function() {
                    documentsToBeSigned[0].signed = true;
                    $modal.modal('hide');

                    // change label and class of trigger element
                    el.innerHTML = 'Upravit podpis';
                    el.classList.remove('btn-primary');
                    el.classList.add('btn-default');

                    // trigger custom event
                    $(el).closest('form').trigger('retention.document.signed');
                }).fail(function() {
                    $modal.find('.modal-content').removeClass('loading');

                    // handle error state
                    if (!errorMesssageVisible) {
                        Message($modal.find('.modal-body'), 'signFileError', true);
                        errorMesssageVisible = true;
                    }
                });
            }
        });

        // modal events
        $modal
            .on('show.bs.modal', function() {
                const document = window.documentsToBeSigned[0];
                const time = new Date().getTime();
                const documentUrl = `${config.offerPage.getFileForSignUrl + document.key}&t=${time}`;

                if (document) {
                    $modal.find('.modal-body .document-wrapper').prepend(`<img src="${documentUrl}" alt="" />`);
                }
            })
            .on('shown.bs.modal', function() {
                if ($.fn.jSignature) {
                    // jSignature initialization
                    $signature.jSignature();
                } else {
                    console.error('jSignature failed to initialize => the plugin is missing.');
                }
            })
            .on('hidden.bs.modal', function() {
                if ($.fn.jSignature) {
                    // when modal is closed, destroy the current signature
                    $signature.jSignature('destroy');
                }

                // remove previously added class
                $modal.find('.modal-content').removeClass('loading');

                // remove previously added image => if modal is opened again, request new image
                $modal.find('.modal-body .document-wrapper img').remove();
            });
    })(jQuery);
}
