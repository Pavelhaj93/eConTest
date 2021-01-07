import React from 'react'
import { render, screen, waitFor } from '@testing-library/react'
import { FileUpload } from './'
import { createFileFromMockFile } from '../../../mocks/createFile'
import { UserDocument } from '@stores'
import { FileError } from '@types'

const mockSuccessfullResponse = {
  id: 'testId123',
  uploaded: true,
}

describe('FileUpload', () => {
  it('renders cancel button', () => {
    const file = createFileFromMockFile({
      name: 'document.txt',
      body: 'content',
      mimeType: 'text/plain',
    })

    const document = new UserDocument(file, 'testId123')

    render(
      <FileUpload
        file={document.file}
        labels={{}}
        onRemove={jest.fn}
        uploading={true}
        uploadHandler={() => Promise.resolve(mockSuccessfullResponse)}
      />,
    )

    const button = screen.getByRole('button')
    expect(button).toBeInTheDocument()
  })

  it('renders error message from API', async () => {
    const file = createFileFromMockFile({
      name: 'document.txt',
      body: 'content',
      mimeType: 'text/plain',
    })

    const document = new UserDocument(file, 'testId123')
    const errorMessage = 'Document failed to upload'

    render(
      <FileUpload
        file={document.file}
        labels={{}}
        onRemove={jest.fn}
        uploading={true}
        uploadHandler={() => Promise.resolve({ uploaded: false })}
        error={errorMessage}
      />,
    )

    await waitFor(() => {
      const message = screen.getByText(`(${errorMessage})`)
      expect(message).toBeInTheDocument()
    })
  })

  it('translates and renders custom error message', async () => {
    const file = createFileFromMockFile({
      name: 'document.txt',
      body: 'content',
      mimeType: 'text/plain',
    })

    const labels = {
      fileExceedSizeError: 'Custom error message',
    }

    const document = new UserDocument(file, 'testId123')

    render(
      <FileUpload
        file={document.file}
        labels={labels}
        onRemove={jest.fn}
        uploading={true}
        uploadHandler={() => Promise.resolve({ uploaded: false })}
        error={FileError.SIZE_EXCEEDED}
      />,
    )

    await waitFor(() => {
      const message = screen.getByText(`(${labels.fileExceedSizeError})`)
      expect(message).toBeInTheDocument()
    })
  })
})
