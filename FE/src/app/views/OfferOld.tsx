// import React, { useEffect, useState, useCallback, useRef, FormEvent, useMemo } from 'react'
// import { CommodityProductType, OfferType, View } from '@types'
// import { observer } from 'mobx-react-lite'
// import { OfferStore } from '@stores'
// import { Alert, Button, Col, Form, Row, Table } from 'react-bootstrap'
// import classNames from 'classnames'
// import Media from 'react-media'
// import {
//   Box,
//   BoxHeading,
//   ConfirmationModal,
//   UnfinishedOfferModal,
//   DocumentLink,
//   FileUpload,
//   FormCheckWrapper,
//   Gift,
//   Icon,
//   SignatureModal,
//   SignButton,
//   UploadZone,
// } from '@components'
// import { breakpoints, colors } from '@theme'
// import { useKeepAlive, useLabels, useUnload } from '@hooks'
// import { OfferStoreContext } from '@context'
// import { isIE11, parseUrl } from '@utils'

// type SignatureModalType = {
//   id: string
//   show: boolean
// }

// export const OfferCopy: React.FC<View> = observer(
//   ({
//     guid,
//     offerUrl,
//     cancelDialog,
//     labels,
//     keepAliveUrl,
//     getFileUrl,
//     thumbnailUrl,
//     signFileUrl,
//     timeout,
//     uploadFileUrl,
//     removeFileUrl,
//     errorPageUrl,
//     allowedContentTypes,
//     maxFileSize,
//     maxGroupFileSize,
//     acceptOfferUrl,
//     thankYouPageUrl,
//     sessionExpiredPageUrl,
//     backToOfferUrl,
//     suppliers,
//     version = 3,
//   }) => {
//     const [store] = useState(() => new OfferStore(OfferType.NEW, offerUrl, guid))
//     const [signatureModalProps, setSignatureModalProps] = useState<SignatureModalType>({
//       id: '',
//       show: false,
//     })
//     const [confirmationModal, setConfirmationModal] = useState<boolean>(false)
//     const t = useLabels(labels)
//     const formRef = useRef<HTMLFormElement>(null)

//     // keep session alive
//     useKeepAlive(30 * 1000, keepAliveUrl ? parseUrl(keepAliveUrl, { guid }) : keepAliveUrl)

//     useEffect(() => {
//       store.errorPageUrl = errorPageUrl
//       store.sessionExpiredPageUrl = sessionExpiredPageUrl
//       store.fetchOffer(timeout)

//       // set correct upload document URL if provided
//       if (uploadFileUrl) {
//         store.uploadDocumentUrl = uploadFileUrl
//       }

//       if (removeFileUrl) {
//         store.removeDocumentUrl = removeFileUrl
//       }

//       if (maxGroupFileSize) {
//         store.maxUploadGroupSize = maxGroupFileSize
//       }

//       if (acceptOfferUrl) {
//         store.acceptOfferUrl = acceptOfferUrl
//       }

//       if (cancelDialog) {
//         store.cancelOfferUrl = cancelDialog.cancelOfferUrl
//       }

//       if (suppliers) {
//         store.isSupplierMandatory = true
//       }
//       // eslint-disable-next-line react-hooks/exhaustive-deps
//     }, [])

//     // show warning to user when trying to refresh or leave the page once he did some changes
//     useUnload(ev => {
//       if (
//         store.isOfferDirty &&
//         !store.isAccepting &&
//         !store.forceReload &&
//         !store.isUnfinishedOfferModalOpen
//       ) {
//         ev.preventDefault()
//         ev.returnValue = ''
//       }
//     })

//     const openSignatureModal = useCallback((id: string) => {
//       setSignatureModalProps({
//         id,
//         show: true,
//       })
//     }, [])

//     // Since IE11 doesn't support a `download` attribute on link, here I manually change the `forceReload`
//     // property so browser can refresh and download the file.
//     const handleDownload = useCallback(() => {
//       if (isIE11()) {
//         store.forceReload = true
//       }
//     }, [store])

//     // each commodity has a different background color
//     const benefitsBoxBgColor = useMemo(() => {
//       switch (store.benefits?.commodityProductType) {
//         case CommodityProductType.ELECTRICITY:
//           return 'purple-light'

//         case CommodityProductType.GAS:
//           return 'blue'

//         default:
//           return 'purple-light'
//       }
//     }, [store.benefits?.commodityProductType])

//     const renderInfoElement = (value: string | undefined, className?: string) =>
//       value && (
//         <div className={classNames('text-center mt-4', className)}>
//           <Icon
//             name="info-circle"
//             size={40}
//             color={colors.gray100}
//             className="d-block mx-auto mb-3"
//           />
//           <div className="editorial-content" dangerouslySetInnerHTML={{ __html: value }} />
//         </div>
//       )

//     return (
//       <OfferStoreContext.Provider value={store}>
//         {/* error state */}
//         {store.error && (
//           <Alert variant="danger">
//             <h3>{t('appUnavailableTitle')}</h3>
//             <div
//               data-testid="errorMessage"
//               className="editorial-content"
//               dangerouslySetInnerHTML={{ __html: store.errorMessage || t('appUnavailableText') }}
//             />
//           </Alert>
//         )}
//         {/* /error state */}

//         <form
//           action={acceptOfferUrl}
//           method="post"
//           ref={formRef}
//           className={classNames({
//             'bg--gray-10': store.isLoading,
//             loading: store.isLoading,
//             'd-none': store.error, // hide the whole form if there is an error
//           })}
//           onSubmit={(ev: FormEvent) => ev.preventDefault()}
//         >
//           {/* summary / perex box */}
//           {store.perex && (
//             <Box backgroundColor="gray-80">
//               <BoxHeading>{store.perex.title}</BoxHeading>
//               {store.perex.params.length > 0 && (
//                 <Table
//                   size="sm"
//                   borderless
//                   data-testid="summaryTable"
//                   className="table-two-columns"
//                 >
//                   <tbody>
//                     {store.perex.params.map(({ title, value }, idx) => (
//                       <tr key={idx}>
//                         <th scope="row">{title}:</th>
//                         <td>{value}</td>
//                       </tr>
//                     ))}
//                   </tbody>
//                 </Table>
//               )}
//             </Box>
//           )}
//           {/* /summary / perex box */}

//           {/* benefits box */}
//           {store.benefits && (
//             <Box backgroundColor={benefitsBoxBgColor} className="text-white">
//               <BoxHeading>{store.benefits.title}</BoxHeading>
//               {store.benefits.params.length > 0 && (
//                 <Row as="ul" className="justify-content-center list-unstyled mb-0">
//                   {store.benefits.params.map(({ value }, idx) => (
//                     <Col as="li" key={idx} xs={12} sm={6} lg={4} className="my-3 text-center">
//                       <Icon name="check-circle" size={40} color={colors.white} />
//                       <span className="d-block mt-2 font-weight-bold">{value}</span>
//                     </Col>
//                   ))}
//                 </Row>
//               )}
//             </Box>
//           )}
//           {/* /benefits box */}

//           {/* box with documents to be accepted or signed */}
//           {(store.documents.acceptance?.accept || store.documents.acceptance?.sign) && (
//             <>
//               <Box>
//                 <BoxHeading>
//                   {store.documents.acceptance?.sectionInfo?.title ||
//                     store.documents.acceptance.title}
//                 </BoxHeading>
//                 <div
//                   className="editorial-content text-center"
//                   dangerouslySetInnerHTML={{
//                     __html:
//                       store.documents.acceptance?.sectionInfo?.text ||
//                       store.documents.acceptance.text,
//                   }}
//                 />
//               </Box>
//               {(store.documents.acceptance?.sectionInfo?.note || store.documents.description) && (
//                 <div
//                   className="py-1 px-3 editorial-content text-center mb-4"
//                   dangerouslySetInnerHTML={{
//                     __html:
//                       store.documents.acceptance?.sectionInfo?.note ||
//                       store.documents?.description ||
//                       '',
//                   }}
//                 />
//               )}
//               <Box data-testid="boxDocumentsToBeAccepted">
//                 {store.documentsToBeAccepted.length > 0 && (
//                   <>
//                     <BoxHeading>{store.documents.acceptance?.accept?.title}</BoxHeading>
//                     <div
//                       className="my-4 text-center editorial-content"
//                       dangerouslySetInnerHTML={{
//                         __html: store.documents.acceptance?.accept?.subTitle ?? '',
//                       }}
//                     />
//                     <div className="mb-2">
//                       <Button
//                         variant="link"
//                         onClick={() => store.acceptAllDocuments(store.documentsToBeAccepted)}
//                         aria-pressed={store.allDocumentsAreAccepted}
//                       >
//                         {t('acceptAll')}
//                       </Button>
//                     </div>
//                     {store.documentsToBeAccepted.map(({ key, prefix, accepted, label, note }) => (
//                       <>
//                         <FormCheckWrapper
//                           key={key}
//                           type="checkbox"
//                           name="acceptedDocuments"
//                           id={`document-${key}`}
//                           value={key}
//                           checked={accepted !== undefined ? accepted : false}
//                           onChange={() => store.acceptDocument(key)}
//                         >
//                           <Form.Check.Label>
//                             <span className="mr-1">{prefix}</span>
//                             <DocumentLink
//                               url={parseUrl(`${getFileUrl}/${key}`, { guid })}
//                               label={label}
//                               onClick={handleDownload}
//                             />
//                           </Form.Check.Label>
//                         </FormCheckWrapper>
//                         {/* info text */}
//                         {renderInfoElement(note, 'mb-5')}
//                       </>
//                     ))}
//                   </>
//                 )}

//                 {store.documentsToBeSigned.length > 0 && (
//                   <>
//                     <BoxHeading>{store.documents.acceptance.sign?.title}</BoxHeading>
//                     <div
//                       className="editorial-content text-center my-4"
//                       dangerouslySetInnerHTML={{
//                         __html: store.documents.acceptance?.sign?.subTitle ?? '',
//                       }}
//                     />
//                     {store.documentsToBeSigned.map(({ key, prefix, label, accepted, note }) => (
//                       <>
//                         <div key={key} className="form-item-wrapper mb-3">
//                           <div className="like-custom-control-label">
//                             {accepted && (
//                               <Icon
//                                 name="check-circle"
//                                 size={36}
//                                 color={colors.green}
//                                 className="form-item-wrapper__icon mr-2"
//                               />
//                             )}
//                             <span>
//                               {prefix}{' '}
//                               <DocumentLink
//                                 url={parseUrl(`${getFileUrl}/${key}?t=${new Date().getTime()}`, {
//                                   guid,
//                                 })}
//                                 label={label}
//                                 onClick={handleDownload}
//                                 noIcon
//                               />
//                             </span>
//                             <SignButton
//                               className="d-none d-sm-block"
//                               signed={accepted}
//                               onClick={() => openSignatureModal(key)}
//                               labelSign={t('signatureBtn')}
//                               labelEdit={t('signatureEditBtn')}
//                               descriptionId={'signBtnDescription'}
//                               showLabelEdit={false}
//                             />
//                           </div>
//                           <Media query={{ maxWidth: breakpoints.smMax }}>
//                             {matches =>
//                               matches && (
//                                 <SignButton
//                                   className="btn-block-mobile mt-3"
//                                   signed={accepted}
//                                   onClick={() => openSignatureModal(key)}
//                                   labelSign={t('signatureBtn')}
//                                   labelEdit={t('signatureEditBtn')}
//                                   descriptionId={'signBtnDescription'}
//                                   showLabelEdit={true}
//                                 />
//                               )
//                             }
//                           </Media>
//                         </div>
//                         {/* info text */}
//                         {renderInfoElement(note, 'mb-4')}
//                       </>
//                     ))}
//                     <div
//                       id="signBtnDescription"
//                       className="editorial-content text-muted small"
//                       dangerouslySetInnerHTML={{
//                         __html: t('signatureNote'),
//                       }}
//                       aria-hidden="true"
//                     />
//                   </>
//                 )}
//               </Box>
//             </>
//           )}
//           {/* /box with documents to be accepted or signed */}

//           {/* services box */}
//           {store.documents.other?.services && (
//             <>
//               <h2 className="mt-5 text-center">
//                 {store.documents.other.services?.sectionInfo?.title ||
//                   store.documents.other.services.title}
//               </h2>
//               {store.documents.other.services?.sectionInfo && (
//                 <Box>
//                   <BoxHeading>{store.documents.other.services?.subTitle}</BoxHeading>
//                   {store.documents.other.services?.sectionInfo?.text ? (
//                     <div
//                       className="editorial-content text-center"
//                       dangerouslySetInnerHTML={{
//                         __html: store.documents.other.services?.sectionInfo?.text,
//                       }}
//                     />
//                   ) : (
//                     store.documents.other.services.params.length > 0 && (
//                       <Table size="sm" className="table-two-columns" borderless>
//                         <tbody>
//                           {store.documents.other.services.params.map(({ title, value }, idx) => (
//                             <tr key={idx}>
//                               <th scope="row">{title}:</th>
//                               <td>{value}</td>
//                             </tr>
//                           ))}
//                         </tbody>
//                       </Table>
//                     )
//                   )}
//                 </Box>
//               )}
//               {store.documents.other.services?.sectionInfo?.note && (
//                 <div
//                   className="py-1 px-3 editorial-content text-center mb-4"
//                   dangerouslySetInnerHTML={{
//                     __html: store.documents.other.services?.sectionInfo?.note,
//                   }}
//                 />
//               )}
//               <Box>
//                 {store.documents.other.services.subTitle &&
//                   (store.documents.other.services.params.length > 0 ||
//                     store.documents.other.services.arguments.length > 0) && (
//                     <BoxHeading>{store.documents.other.services.subTitle}</BoxHeading>
//                   )}

//                 {store.documents.other.services.params.length > 0 && (
//                   <Table size="sm" className="table-two-columns" borderless>
//                     <tbody>
//                       {store.documents.other.services.params.map(({ title, value }, idx) => (
//                         <tr key={idx}>
//                           <th scope="row">{title}:</th>
//                           <td>{value}</td>
//                         </tr>
//                       ))}
//                     </tbody>
//                   </Table>
//                 )}

//                 {store.documents.other.services.arguments.length > 0 && (
//                   <Box backgroundColor="blue-green-light">
//                     <Row
//                       as="ul"
//                       className="justify-content-center list-unstyled mb-0"
//                       aria-label={t('productBenefits')}
//                     >
//                       {store.documents.other.services.arguments.map(({ value }, idx) => (
//                         <Col as="li" key={idx} xs={12} sm={6} lg={4} className="my-3 text-center">
//                           <Icon name="check-circle" size={40} color={colors.white} />
//                           <span className="d-block mt-2 font-weight-bold">{value}</span>
//                         </Col>
//                       ))}
//                     </Row>
//                   </Box>
//                 )}

//                 {store.documents.other.services.subTitle2 && (
//                   <BoxHeading>{store.documents.other.services.subTitle2}</BoxHeading>
//                 )}
//                 <div
//                   className="text-center editorial-content mt-2 mb-4"
//                   dangerouslySetInnerHTML={{ __html: store.documents.other.services.text }}
//                 />
//                 <div className="mb-2">
//                   <Button
//                     variant="link"
//                     onClick={() => store.acceptAllDocuments(store.documentsServices)}
//                     aria-pressed={store.documentsServices.every(d => d.accepted)}
//                   >
//                     {t('acceptAll')}
//                   </Button>
//                 </div>
//                 {store.documentsServices.map(({ key, prefix, label, accepted, note }) => (
//                   <>
//                     <FormCheckWrapper
//                       key={key}
//                       type="checkbox"
//                       name="commoditiesDocuments"
//                       id={`document-${key}`}
//                       value={key}
//                       checked={accepted !== undefined ? accepted : false}
//                       onChange={() => store.acceptDocument(key)}
//                     >
//                       <Form.Check.Label>
//                         <span className="mr-1">{prefix}</span>
//                         <DocumentLink
//                           url={parseUrl(`${getFileUrl}/${key}`, { guid })}
//                           label={label}
//                           onClick={handleDownload}
//                         />
//                       </Form.Check.Label>
//                     </FormCheckWrapper>
//                     {/* info text */}
//                     {renderInfoElement(note, 'mb-5')}
//                   </>
//                 ))}
//                 {/* info text */}
//                 {renderInfoElement(store.documents.other.services.note)}
//               </Box>
//             </>
//           )}
//           {/* /services box */}

//           {/* products box */}
//           {store.documents.other?.products && (
//             <>
//               <h2 className="mt-5 text-center">
//                 {store.documents.other.products?.sectionInfo?.title ||
//                   store.documents.other.products.title}
//               </h2>
//               <Box>
//                 <BoxHeading>{store.documents.other.products.subTitle}</BoxHeading>
//                 {store.documents.other.products?.sectionInfo?.text ? (
//                   <div
//                     className="editorial-content text-center"
//                     dangerouslySetInnerHTML={{
//                       __html: store.documents.other.products?.sectionInfo?.text,
//                     }}
//                   />
//                 ) : (
//                   store.documents.other.products.params.length > 0 && (
//                     <Table size="sm" className="table-two-columns" borderless>
//                       <tbody>
//                         {store.documents.other.products.params.map(({ title, value }, idx) => (
//                           <tr key={idx}>
//                             <th scope="row">{title}:</th>
//                             <td>{value}</td>
//                           </tr>
//                         ))}
//                       </tbody>
//                     </Table>
//                   )
//                 )}

//                 {version === 2 && store.documents.other.products.arguments.length > 0 && (
//                   <Box backgroundColor="blue-green-light">
//                     <Row
//                       as="ul"
//                       className="justify-content-center list-unstyled mb-0"
//                       aria-label={t('productBenefits')}
//                     >
//                       {store.documents.other.products.arguments.map(({ value }, idx) => (
//                         <Col as="li" key={idx} xs={12} sm={6} lg={4} className="my-3 text-center">
//                           <Icon name="check-circle" size={40} color={colors.white} />
//                           <span className="d-block mt-2 font-weight-bold">{value}</span>
//                         </Col>
//                       ))}
//                     </Row>
//                   </Box>
//                 )}
//               </Box>
//               {version === 3 && store.documents.other.products?.sectionInfo?.note && (
//                 <div
//                   className="py-1 px-3 editorial-content text-center mb-4"
//                   dangerouslySetInnerHTML={{
//                     __html: store.documents.other.products?.sectionInfo?.note,
//                   }}
//                 />
//               )}
//               <Box className={version === 2 ? 'mt-n4' : ''}>
//                 <BoxHeading>{store.documents.other.products.subTitle2}</BoxHeading>
//                 <div
//                   className="text-center editorial-content my-4"
//                   dangerouslySetInnerHTML={{ __html: store.documents.other.products.text }}
//                 />
//                 <div className="mb-2">
//                   <Button
//                     variant="link"
//                     onClick={() => store.acceptAllDocuments(store.documentsProducts)}
//                     aria-pressed={store.documentsProducts.every(d => d.accepted)}
//                   >
//                     {t('acceptAll')}
//                   </Button>
//                 </div>
//                 {store.documentsProducts.map(({ key, prefix, label, accepted, note }) => (
//                   <>
//                     <FormCheckWrapper
//                       key={key}
//                       type="checkbox"
//                       name="commoditiesDocuments"
//                       id={`document-${key}`}
//                       value={key}
//                       checked={accepted !== undefined ? accepted : false}
//                       onChange={() => store.acceptDocument(key)}
//                     >
//                       <Form.Check.Label>
//                         <span className="mr-1">{prefix}</span>
//                         <DocumentLink
//                           url={parseUrl(`${getFileUrl}/${key}`, { guid })}
//                           label={label}
//                           onClick={handleDownload}
//                         />
//                       </Form.Check.Label>
//                     </FormCheckWrapper>
//                     {/* info text */}
//                     {renderInfoElement(note, 'mb-5')}
//                   </>
//                 ))}
//                 {/* info text */}
//                 {renderInfoElement(store.documents.other.products.note)}
//               </Box>
//             </>
//           )}
//           {/* /products box */}

//           {/**
//            * submit zone
//            * Hide the zone if there is an error or we didn't receive any documents.
//            */}
//           <div
//             className={classNames({
//               'd-none': (!store.isLoading && store.error) || !store.offerFetched,
//             })}
//           >
//             <h2 className="mt-5 text-center">{t('acceptOfferTitle')}</h2>
//             <Box>
//               {suppliers && (
//                 <Form.Group>
//                   <Form.Label htmlFor="supplier">{suppliers.label}</Form.Label>
//                   <Form.Control
//                     as="select"
//                     id="supplier"
//                     value={store.supplier}
//                     onChange={event => store.selectSupplier(event.target.value)}
//                   >
//                     {[{ label: '', value: '' }, ...suppliers.items].map(({ label, value }, idx) => (
//                       <option key={idx} value={value}>
//                         {label}
//                       </option>
//                     ))}
//                   </Form.Control>
//                 </Form.Group>
//               )}
//               <div className="text-center">
//                 <div
//                   className="editorial-content mb-3"
//                   dangerouslySetInnerHTML={{ __html: t('acceptOfferHelptext') }}
//                 />
//                 <Button
//                   variant="secondary"
//                   type="submit"
//                   onClick={() => setConfirmationModal(true)}
//                   disabled={!store.isOfferReadyToAccept}
//                 >
//                   {t('submitBtn')}
//                 </Button>
//                 {/* If a user wants to start again, the following button will be visible */}
//                 {cancelDialog && (
//                   <Button
//                     className="ml-3"
//                     variant="outline-primary"
//                     type="submit"
//                     onClick={() => store.setIsUnfinishedOfferModalOpen(true)}
//                   >
//                     {t('startOver')}
//                   </Button>
//                 )}
//               </div>
//             </Box>
//           </div>
//           {/* submit zone */}
//         </form>
//         {backToOfferUrl && (
//           <Button
//             variant="link"
//             className="underline text-black m-auto w-content d-flex"
//             href={backToOfferUrl}
//           >
//             {t('backToOffer', 'Zpět na nabídku')}
//           </Button>
//         )}
//         <SignatureModal
//           {...signatureModalProps}
//           onClose={() => setSignatureModalProps({ show: false, id: '' })}
//           labels={labels}
//           thumbnailUrl={thumbnailUrl ?? ''}
//           signFileUrl={signFileUrl ?? ''}
//           guid={guid}
//         />

//         <ConfirmationModal
//           show={confirmationModal && !store.isUnfinishedOfferModalOpen}
//           onClose={() => setConfirmationModal(false)}
//           labels={labels}
//           thankYouPageUrl={thankYouPageUrl}
//           cancelDialog={!!cancelDialog}
//           openUnfinishedModal={() => store.setIsUnfinishedOfferModalOpen(true)}
//         />

//         <UnfinishedOfferModal
//           show={store.isUnfinishedOfferModalOpen}
//           onClose={() => store.setIsUnfinishedOfferModalOpen(false)}
//           redirectUrl={cancelDialog?.redirectUrl ?? ''}
//           labels={labels}
//         />
//       </OfferStoreContext.Provider>
//     )
//   },
// )
