import { Box, BoxHeading, DocumentLink, FormCheckWrapper } from '@components'
import { useLabels } from '@hooks'
import { NewOfferResponseCopy } from '@types'
import { parseUrl } from '@utils'
import React, { FC, Fragment } from 'react'
import { Button, Form } from 'react-bootstrap'
import InfoElement from './InfoElement'

interface DocsCheckProps {
  t: ReturnType<typeof useLabels>
  headTitle: NewOfferResponseCopy.Header['title']
  headParams: NewOfferResponseCopy.Header['params']
  headText: NewOfferResponseCopy.Header['text']
  docGroupsToBeChecked: NewOfferResponseCopy.File[][]
  allDocumentsAreChecked: boolean
  checkDocumentsGroup: (files: NewOfferResponseCopy.Docs['files']) => void
  checkDocument: (key: string) => void
  docsTitle: NewOfferResponseCopy.Docs['title']
  docsFiles: NewOfferResponseCopy.Docs['files']
  docsText: NewOfferResponseCopy.Docs['text']
  handleDownload: () => void
  getFileUrl: string | undefined
  guid: string
}

const DocsCheck: FC<DocsCheckProps> = ({
  t,
  headTitle,
  headParams,
  headText,
  docGroupsToBeChecked,
  allDocumentsAreChecked,
  checkDocumentsGroup,
  checkDocument,
  docsTitle,
  docsFiles,
  docsText,
  handleDownload,
  getFileUrl,
  guid,
}) => {
  return (
    <Fragment>
      <Box>
        <BoxHeading>{headTitle}</BoxHeading>
        {headParams && (
          <div
            className="editorial-content text-center"
            dangerouslySetInnerHTML={{
              __html: headParams.map(param => param.title + ' ' + param.value).join(' '),
            }}
          />
        )}
      </Box>
      {/* TODO: bylo tu jeste store.documents.description  zkontrolovat jestli to nechybi */}
      {headText && (
        <div
          className="py-1 px-3 editorial-content text-center mb-4"
          dangerouslySetInnerHTML={{
            __html: headText || '',
          }}
        />
      )}
      <Box data-testid="boxDocumentsToBeAccepted">
        {docGroupsToBeChecked.length > 0 && (
          <>
            <BoxHeading>{docsTitle}</BoxHeading>
            <div
              className="my-4 text-center editorial-content"
              dangerouslySetInnerHTML={{
                __html: docsText ?? '',
              }}
            />
            <div className="mb-2">
              <Button
                variant="link"
                onClick={() => checkDocumentsGroup(docsFiles ?? [])}
                aria-pressed={allDocumentsAreChecked}
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
                  checked={accepted}
                  onChange={() => checkDocument(key)}
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
          </>
        )}
      </Box>
    </Fragment>
  )
}

export default DocsCheck
