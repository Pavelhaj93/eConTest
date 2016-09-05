export default function Message(container, messageType) {
    !(container instanceof jQuery) && (container = $(container));

    /* All the messages */
    const messages = {
        appUnavailable: [
            '<h3>Nedostupnost aplikace</h3>',
            '<p>Omlouváme se Vám za nedostupnost aplikace. Příčinou může být právě probíhající odstávka. Prosím opakujte akci později.</p>'
        ]
    };

    /* Get the required message */
    const message = messages[messageType].join('');

    /* Append to container */
    container.append(message);
}