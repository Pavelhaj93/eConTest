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
    /* Stop when no messages are passed from Back-end */
    if (!messages) { return; }

    !(container instanceof jQuery) && (container = $(container));

    /* Get the required message */
    const message = messages[name].join('');

    /* Append to container */
    container.append(message);
}
