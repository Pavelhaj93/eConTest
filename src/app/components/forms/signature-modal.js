export default function SignatureModal(el, config) {
    (function($) {
        const target = $(el).data('target');
        const $modal = $(target);
        const $signature = $modal.find('.js-signature');
        const $clearBtn = $modal.find('.js-signature-clear-btn');
        const $saveBtn = $modal.find('.js-signature-save-btn');

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

            if ($.fn.jSignature) {
                $modal.find('.modal-content').addClass('loading');

                // get PNG as base 64
                const signatureData = $signature.jSignature('getData', 'image')[1];

                // id of document that is being signed
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

                    // change label of trigger element
                    el.innerHTML = 'Upravit podpis';

                    // trigger custom event
                    $(el).closest('form').trigger('retention.document.signed');
                }).fail(function() {
                    $modal.find('.modal-content').removeClass('loading');
                    console.log('error');
                });
            }
        });

        // modal events
        $modal
            .on('shown.bs.modal', function() {
                if ($.fn.jSignature) {
                    // jSignature inicialization
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
            });
    })(jQuery);
}
