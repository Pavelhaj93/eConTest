import Alert from '../../components/alert';
import { alertTypes } from '../../components/alert';
import { messages } from '../messages';
import url from 'url';
import trim from '../../helpers/trim';

export default function Auth(form) {
    (function($) {

        window.addEventListener('load', () => {
            const queryMessage = url.parse(window.location.href, true).query;

            if (queryMessage) {
                Object.keys(queryMessage).forEach((messageType) => {

                    /* Forbid output of unknown message types */
                    if (alertTypes.includes(messageType)){
                        const messageSlug = queryMessage[messageType];

                        if (messages.hasOwnProperty(messageSlug)) {
                            Alert(messages[messageSlug], messageType, status);
                        }
                    }

                });
            }
        });

        /* Inputs */
        const birth = form.querySelector('input[name="birth"]');
        const additionalInput = form.querySelector('input[name="additional"]');
        const submitBtn = form.querySelector('button[type="submit"]');

        /* Status form */
        const status = $(form.querySelector('.status'));

        /* Shallow validation: Check only for the empty fields */
        function validate() {
            const required = Array.from(form.querySelectorAll('[required]'));
            let result = true;
            let invalid = [];

            /* Validate the inputs */
            required.map((input) => {
                input.classList.remove('invalid');

                if (!input.value.trim()) {
                    input.classList.add('invalid');
                    invalid.push(input);
                    result = false;
                }
            });

            return result;
        }

        /* Print invalid date error */
        function invalidMessage() {
            return Alert(messages.validationError, 'error', status);
        }

        /* Handle submit */
        submitBtn.addEventListener('click', (e) => {
            e.preventDefault();

            /* Empty status box */
            $(status).empty();

            /* Validate the form for missed fields */
            const validated = validate();

            if (validated) {
                /* Validate customer's birth date */
                const validBirth = (trim(birth.value) == trim(dob));
                const validAdditional = (trim(additionalInput.value) == trim(additional));

                if (validBirth && validAdditional) {
                    console.warn('SEND REQUEST');

                    /* Validation request */
                    // $.ajax({
                    //     type: '',
                    //     dataType: 'jsonp',
                    //     url: 'http://google.com',
                    //     data: {
                    //         additional: additionalInput.value
                    //     },
                        
                    //     /* When success, redirect to the Offer page */
                    //     success: (response) => {
                    //         if (response.success) {
                    //             console.warn('REDIRECT TO: Offer');
                    //         } else {
                    //             /* Display validation error message */
                    //             invalidMessage();
                    //         }
                    //         console.log(response);
                    //     },

                    //     /* ERROR: No response from the server */
                    //     error: (e) => {
                    //         /* Redirect to System error page */
                    //     }
                    // });

                } else {
                    /* ERROR: Invalid data */
                    invalidMessage();
                }
            } else {
                /* ERROR: Missing required fields */
                Alert(messages.requiredFields, 'error', status);
            }

        });

    })(jQuery);
}
