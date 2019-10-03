import isMobileDevice from '../helpers/is-mobile-device';

const addLeadingZero = value => (`0${value}`).slice(-2);
const datePickerFullClass = 'full';

export default function DateInput(container) {
    container = $(container);
    let isActive = false;

    // Bug: jQuery datepicker not hiding on iOS when clicked outside
    // Fix: Add a dummy click listener to catch the click event
    // https://stackoverflow.com/questions/28006056/jquery-datepicker-not-hiding-on-ios-when-clicked-on-background
    document.querySelector('.main').addEventListener('click', () => {});

    /* For mobile device display native datepicker, otherwise datepicker component */

    // Disable the functionality due client's request. Keep it commented for the possible future.

    // function handleMobileInputChange(e) {
    //     const { value } = e.target;
    //     if (!value) return;

    //     const date = value.split('-');
    //     const day = date[2];
    //     const month = date[1];
    //     const year = date[0];
    //     const finalDate = `${day}. ${month}. ${year}`;

    //     container.val(finalDate);
    // }

    //
    // if (isMobileDevice()) {
    //     const dateInput = container.clone();
    //     const id = container.attr('id');
    //     const name = container.attr('name');
    //     container.hide();

    //     dateInput
    //         .on('change', handleMobileInputChange)
    //         .attr('type', 'date')
    //         .attr('id', `${id}-mobile`)
    //         .attr('name', `${name}-mobile`)
    //         .insertAfter(container);

    //     container.parent().find('.ui-datepicker-trigger').hide();

    //     /* Placeholder workaround for input type date */
    //     dateInput.on('input', function(){
    //         dateInput.toggleClass(`${datePickerFullClass}`, dateInput.val().length > 0);
    //     });

    //     return;
    // }

    /* Default options for datepicker component */
    const datepickerOptions = {
        language: 'cs',
        format: 'dd. mm. yyyy',
        endDate: '-1d',
        maxViewMode: 2,
        autoclose: true,
        orientation: 'bottom right',
    };

    if (isMobileDevice()) {
        datepickerOptions.showOnFocus = false;

        /* On mobile devices we need to update datepickers's value to match the given date format. */
        container.on('blur', () => {
            container.datepicker('update', container.val());
        });
    }

    /* Initiate on the intput's container for proper absolute/relative relation */
    container.datepicker(datepickerOptions);

    /* Manually append calendar component to the input's container */
    $('.datepicker-dropdown').appendTo(container.parent('div'));

    /* Display calendar on click */
    const trigger = $('.ui-datepicker-trigger');
    trigger.on('click', (e) => {
        e.preventDefault();

        container.datepicker('show');
    });
}
