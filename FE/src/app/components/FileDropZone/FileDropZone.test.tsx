import React from 'react'
import { render, screen } from '@testing-library/react'
import { FileDropZone } from './'

const mockProps = {
  selectFileLabel: 'Select file',
  onFilesAccepted: jest.fn,
}

describe('FileDropZone', () => {
  it('renders', () => {
    render(<FileDropZone {...mockProps} />)
    const selectFileButton = screen.getByText(mockProps.selectFileLabel)

    expect(selectFileButton).toBeInTheDocument()
  })
})
