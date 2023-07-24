import { Box, BoxHeader, DocumentLink, FormCheckWrapper, Icon } from '@components'
import { useLabels } from '@hooks'
import { CommodityProductType, NewOfferResponse } from '@types'
import { parseUrl } from '@utils'
import React, { FC, Fragment, useContext } from 'react'
import { Button, Form } from 'react-bootstrap'
import InfoElement from './InfoElement'
import { OfferStoreContext } from '@context'
import { OfferStore } from '@stores'
import { observer } from 'mobx-react-lite'
import { getColorByCommodityType, getCommodityTitle } from '../utils/strings'
import Perex from './Perex'

interface DocsCheckProps {
  t: ReturnType<typeof useLabels>
  type?: CommodityProductType
  headerTitle: NewOfferResponse.Header['title']
  headTitle: NewOfferResponse.Header['title']
  headText: NewOfferResponse.Header['text']
  bodyText: NewOfferResponse.Body['text']
  docsPerex?: NewOfferResponse.Docs['perex']
  docsTitle: NewOfferResponse.Docs['title']
  docsFiles: NewOfferResponse.Docs['files']
  docsText: NewOfferResponse.Docs['text']
  docsParams: NewOfferResponse.Docs['params']
  bodyNote: NewOfferResponse.Body['note']
  handleDownload: () => void
  getFileUrl: string
  guid: string
}

const DocsCheck: FC<DocsCheckProps> = observer(
  ({
    t,
    type,
    headerTitle,
    headTitle,
    bodyText,
    headText,
    docsPerex,
    docsTitle,
    docsFiles,
    docsText,
    docsParams,
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
        {headerTitle && (
          <BoxHeader>
            <h2 className="text-center text-white mb-0">{headerTitle}</h2>
          </BoxHeader>
        )}
        {headTitle && (
          <Box className="mb-4">
            <h3 className="text-center font-weight-bolder">{headTitle}</h3>
            {headText && (
              <div
                className="editorial-content text-center font-weight-bold "
                dangerouslySetInnerHTML={{
                  __html: headText,
                }}
              />
            )}
          </Box>
        )}
        {bodyText && (
          <div
            className="editorial-content mb-4"
            style={{ lineHeight: 0.8 }}
            dangerouslySetInnerHTML={{
              __html: bodyText,
            }}
          />
        )}

        {type && (
          <BoxHeader backgroundColor={getColorByCommodityType(type)}>
            <Icon name={type} width={30} />
            <h2 className="text-center text-white ml-3">{getCommodityTitle(type, t)}</h2>
          </BoxHeader>
        )}
        {docsPerex && (
          <Perex headerTitle={docsPerex.header.title} bodyParams={docsPerex.body.params} />
        )}
        <Box data-testid="boxDocumentsToBeAccepted" className="mb-4">
          <h3 className="text-center">{docsTitle}</h3>
          {docsText && (
            <div
              className="my-4 text-center editorial-content"
              dangerouslySetInnerHTML={{
                __html: docsText,
              }}
            />
          )}
          {docsParams && (
            <div className="mb-4 text-center">
              {docsParams.map(({ title, value }, index) => (
                <div key={index}>
                  <strong>{title}</strong>
                  <strong className="ml-1">{value}</strong>
                </div>
              ))}
            </div>
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
              {note && <InfoElement value={note} className="mb-4" />}
            </>
          ))}
          {bodyNote && <InfoElement value={bodyNote} className="mb-4" />}
        </Box>
      </Fragment>
    )
  },
)

export default DocsCheck
