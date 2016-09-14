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

        /* Form inputs and status box */
        const submitBtn = form.querySelector('button[type="submit"]');
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

        /* Handle submit */
        submitBtn.onclick = () => {
            console.warn('submitBtn: CLICKED');

            /* Empty status box */
            $(status).empty();

            /* Validate the form for missed fields */
            const validated = validate();

            if (!validate) {
                /* ERROR: Missing required fields */
                Alert(messages.requiredFields, 'error', status);
            }

            return validated;
        };

    })(jQuery);
}
