import { Box, BoxHeading, DocumentLink, FormCheckWrapper } from '@components'
import { useLabels } from '@hooks'
import { NewOfferResponse } from '@types'
import { parseUrl } from '@utils'
import React, { FC, Fragment, useContext } from 'react'
import { Button, Form } from 'react-bootstrap'
import InfoElement from './InfoElement'
import { OfferStoreContext } from '@context'
import { OfferStore } from '@stores'
import { observer } from 'mobx-react-lite'

interface DocsCheckProps {
  t: ReturnType<typeof useLabels>
  headerTitle: NewOfferResponse.Header['title']
  headTitle: NewOfferResponse.Header['title']
  headText: NewOfferResponse.Header['text']
  bodyText: NewOfferResponse.Body['text']
  docsTitle: NewOfferResponse.Docs['title']
  docsFiles: NewOfferResponse.Docs['files']
  docsText: NewOfferResponse.Docs['text']
  bodyNote: NewOfferResponse.Body['note']
  handleDownload: () => void
  getFileUrl: string
  guid: string
}

const DocsCheck: FC<DocsCheckProps> = observer(
  ({
    t,
    headerTitle,
    headTitle,
    bodyText,
    headText,
    docsTitle,
    docsFiles,
    docsText,
    bodyNote,
    handleDownload,
    getFileUrl,
    guid,
  }) => {
    const store = useContext(OfferStoreContext)

    if (!(store instanceof OfferStore)) {
      return null
    }

    return (
      <Fragment>
        {headerTitle && <h2 className="mt-5 text-center">{headerTitle}</h2>}
        {headTitle && (
          <Box>
            <BoxHeading>{headTitle}</BoxHeading>
            {headText && (
              <Box>
                <div
                  className="editorial-content text-center"
                  dangerouslySetInnerHTML={{
                    __html: headText,
                  }}
                />
              </Box>
            )}
          </Box>
        )}
        {bodyText && (
          <div
            className="py-1 px-3 editorial-content text-center mb-4"
            dangerouslySetInnerHTML={{
              __html: bodyText,
            }}
          />
        )}
        <Box data-testid="boxDocumentsToBeAccepted">
          {store.docGroupsToBeChecked.length > 0 && (
            <>
              <BoxHeading>{docsTitle}</BoxHeading>
              {docsText && (
                <div
                  className="my-4 text-center editorial-content"
                  dangerouslySetInnerHTML={{
                    __html: docsText,
                  }}
                />
              )}
              <div className="mb-2">
                <Button
                  variant="link"
                  onClick={() => store.checkDocumentsGroup(docsFiles ?? [])}
                  aria-pressed={store.allDocumentsAreChecked}
                >
                  {t('acceptAll')}
                </Button>
              </div>
              {docsFiles.map(({ key, prefix, accepted, label, note }) => (
                <>
                  <FormCheckWrapper
                    key={key}
                    type="checkbox"
                    name="acceptedDocuments"
                    id={`document-${key}`}
                    value={key}
                    checked={accepted ?? false}
                    onChange={() => store.checkDocument(key)}
                  >
                    <Form.Check.Label>
                      <span className="mr-1">{prefix}</span>
                      <DocumentLink
                        url={parseUrl(`${getFileUrl}/${key}`, { guid })}
                        label={label}
                        onClick={handleDownload}
                      />
                    </Form.Check.Label>
                  </FormCheckWrapper>
                  {/* info text */}
                  {note && <InfoElement value={note} className="mb-5" />}
                </>
              ))}
              {bodyNote && <InfoElement value={bodyNote} className="mb-5" />}
            </>
          )}
        </Box>
      </Fragment>
    )
  },
)

export default DocsCheck
