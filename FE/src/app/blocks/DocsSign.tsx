import { Box, BoxHeading, DocumentLink, Icon, SignButton } from '@components'
import { breakpoints, colors } from '@theme'
import React, { FC, Fragment } from 'react'

import InfoElement from './InfoElement'
import { NewOfferResponseCopy } from '@types'
import { parseUrl } from '@utils'
import { useLabels } from '@hooks'
import Media from 'react-media'

interface DocsSignProps {
  t: ReturnType<typeof useLabels>
  docGroupsToBeSigned: NewOfferResponseCopy.File[][]
  headerTitle: NewOfferResponseCopy.Header['title']
  docsText: NewOfferResponseCopy.Docs['text']
  docsFiles: NewOfferResponseCopy.Docs['files']
  getFileUrl: string | undefined
  guid: string
  handleDownload: () => void
  openSignatureModal: (key: string) => void
}

const DocsSign: FC<DocsSignProps> = ({
  t,
  docGroupsToBeSigned,
  headerTitle,
  docsText,
  docsFiles,
  getFileUrl,
  guid,
  handleDownload,
  openSignatureModal,
}) => {
  return (
    <Fragment>
      {docGroupsToBeSigned.length > 0 && (
        <Box>
          <BoxHeading>{headerTitle}</BoxHeading>
          <div
            className="editorial-content text-center my-4"
            dangerouslySetInnerHTML={{
              __html: docsText ?? '',
            }}
          />
          {docsFiles.map(({ key, prefix, label, accepted, note }) => (
            <>
              <div key={key} className="form-item-wrapper mb-3">
                <div className="like-custom-control-label">
                  {accepted && (
                    <Icon
                      name="check-circle"
                      size={36}
                      color={colors.green}
                      className="form-item-wrapper__icon mr-2"
                    />
                  )}
                  <span>
                    {prefix}{' '}
                    <DocumentLink
                      url={parseUrl(`${getFileUrl}/${key}?t=${new Date().getTime()}`, {
                        guid,
                      })}
                      label={label}
                      onClick={handleDownload}
                      noIcon
                    />
                  </span>
                  <SignButton
                    className="d-none d-sm-block"
                    signed={accepted}
                    onClick={() => openSignatureModal(key)}
                    labelSign={t('signatureBtn')}
                    labelEdit={t('signatureEditBtn')}
                    descriptionId={'signBtnDescription'}
                    showLabelEdit={false}
                  />
                </div>
                <Media query={{ maxWidth: breakpoints.smMax }}>
                  {(matches: any) =>
                    matches && (
                      <SignButton
                        className="btn-block-mobile mt-3"
                        signed={accepted}
                        onClick={() => openSignatureModal(key)}
                        labelSign={t('signatureBtn')}
                        labelEdit={t('signatureEditBtn')}
                        descriptionId={'signBtnDescription'}
                        showLabelEdit={true}
                      />
                    )
                  }
                </Media>
              </div>
              {/* info text */}
              {note && <InfoElement value={note} className="mb-4" />}
            </>
          ))}
          <div
            id="signBtnDescription"
            className="editorial-content text-muted small"
            dangerouslySetInnerHTML={{
              __html: t('signatureNote'),
            }}
            aria-hidden="true"
          />
        </Box>
      )}
    </Fragment>
  )
}

export default DocsSign
