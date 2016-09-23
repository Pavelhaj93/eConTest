import printDocumentsList from './documents-list';
import Message from './message';

export default function AcceptedDocuments(container) {

    // =================================================
    // printDocumentsList(container, documents, {
    //     checked: true,
    //     disabled: true
    // });
    // =================================================

    /* Request the documents */
    $.ajax({
        url: 'http://google.com',

        /* Receive and print the documents */
        success: (response) => {
            const documents = response.documents;

            (documents.length > 0) && printDocumentsList(container, documents, {
                checked: true,
                disabled: true
            });
        },

        /* Show App Unavailable message */
        // error: () => {
        //     container.classList.add('error');
        //     Message(container, 'appUnavailable');
        // },

        /* Remove loading progress */
        complete: () => {
            container.classList.remove('loading');
        }
    });
}