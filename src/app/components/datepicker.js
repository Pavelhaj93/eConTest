export default function DateInput(container) {
    container = $(container);

    /* Initiate on the intput's container for proper absolute/relative relation */
    container.datepicker({
        language: 'cs',
        format: 'dd. mm. yyyy',
        endDate: '-1d',
        maxViewMode: 2,
        autoclose: true,
        orientation: 'bottom right'
    });

    // /* Manually append calendar component to the input's container */
    $('.datepicker-dropdown').appendTo(container.parent('div'));

    /* Display calendar on click */
    const trigger = $('.ui-datepicker-trigger');
    trigger.on('click', (e) => {
        e.preventDefault();

        // const input = $(trigger.siblings('input')[0]);
        container.datepicker('show');
    });
}