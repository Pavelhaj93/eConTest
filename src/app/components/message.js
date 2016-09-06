/*
*
* Message
*
* Messages are used as an output in various AJAX result containers
* to display the warning/error during the AJAX call.
*
* ARGUMENTS:
* container (DOMElement/jQuery) - Container to append the message to.
* name (String) - Message name/type to look in the "messages" Object.
*
*/

export default function Message(container, name) {
    !(container instanceof jQuery) && (container = $(container));

    /* All the messages */
    const messages = {
        appUnavailable: [
            '<h3>Nedostupnost aplikace</h3>',
            '<p>Omlouváme se Vám za nedostupnost aplikace. Příčinou může být právě probíhající odstávka. Prosím opakujte akci později.</p>'
        ]
    };

    /* Get the required message */
    const message = messages[name].join('');

    /* Append to container */
    container.append(message);
}