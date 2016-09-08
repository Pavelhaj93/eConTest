import Alert from '../../components/alert';
import trim from '../../helpers/trim';

export default function Auth(form) {
    (function ($) {

        /* Inputs */
        const birth = form.querySelector('input[name="birth"]');
        const additional = form.querySelector('input[name="additional"]');
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
            return Alert('Ověření Vašich údajů se nezdařilo. Zadejte prosím správné údaje. V případě potíží se prosím obraťte na naši zákaznickou podporu.', 'error', status);
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

                if (validBirth) {
                    console.warn('SEND REQUEST');

                    /* Validation request */
                    $.ajax({
                        type: '',
                        dataType: 'jsonp',
                        url: 'http://google.com',
                        data: {
                            additional: additional.value
                        },
                        
                        /* When success, redirect to the Offer page */
                        success: (response) => {
                            if (response.success) {
                                console.warn('REDIRECT TO: Offer');
                            } else {
                                /* Display validation error message */
                                invalidMessage();
                            }
                            console.log(response);
                        },

                        /* ERROR: No response from the server */
                        error: (e) => {
                            /* Redirect to System error page */
                        }
                    });

                } else {
                    /* ERROR: Invalid data */
                    invalidMessage();
                }
            } else {
                /* ERROR: Missing required fields */
                Alert('Prosím , vyplňte všechna požadovaná pole.', 'error', status);
            }

        });

    })(jQuery);
}
