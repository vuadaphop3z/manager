(function($) {
  $.extend($.summernote.lang, {
    'vi-VN': {
      font: {
        bold: 'In Đậm',
        italic: 'In Nghiêng',
        underline: 'Gạch dưới',
        clear: 'Bỏ định dạng',
        height: 'Chiều cao dòng',
        name: 'Phông chữ',
        strikethrough: 'Gạch ngang',
        subscript: 'Subscript',
        superscript: 'Superscript',
        size: 'Cỡ chữ'
      },
      image: {
        image: 'Hình ảnh',
        insert: 'Chèn',
        resizeFull: '100%',
        resizeHalf: '50%',
        resizeQuarter: '25%',
        floatLeft: 'Trôi về trái',
        floatRight: 'Trôi về phải',
        floatNone: 'Không trôi',
        shapeRounded: 'Hình: Bầu dục',
        shapeCircle: 'Hình: Tròn',
        shapeThumbnail: 'Hình: Hình nhỏ',
        shapeNone: 'Hình: Không',
        dragImageHere: 'Thả ảnh ở vùng này',
        dropImage: 'Thả ảnh hoặc văn bản',
        selectFromFiles: 'Chọn tệp',
        maximumFileSize: 'Kích thước tối đa',
        maximumFileSizeError: 'Vượt quá giới hạn kích thước.',
        url: 'URL',
        remove: 'Xóa',
        original: 'Gốc'
      },
      video: {
        video: 'Video',
        videoLink: 'Link đến Video',
        insert: 'Chèn Video',
        url: 'URL',
        providers: '(YouTube, Vimeo, Vine, Instagram, DailyMotion và Youku)'
      },
      link: {
        link: 'Link',
        insert: 'Chèn Link',
        unlink: 'Gỡ Link',
        edit: 'Sửa',
        textToDisplay: 'Văn bản hiển thị',
        url: 'URL',
        openInNewWindow: 'Mở ở Cửa sổ mới'
      },
      table: {
        table: 'Bảng',
        addRowAbove: 'Thêm dòng bên dưới',
        addRowBelow: 'Thêm dòng phía trên',
        addColLeft: 'Thêm cột bên trái',
        addColRight: 'Thêm cột bên phải',
        delRow: 'Xóa dòng',
        delCol: 'Xóa cột',
        delTable: 'Xóa bảng'
      },
      hr: {
        insert: 'Chèn dòng'
      },
      style: {
        style: 'Kiểu chữ',
        p: 'Chữ thường',
        blockquote: 'Đoạn trích',
        pre: 'Mã Code',
        h1: 'H1',
        h2: 'H2',
        h3: 'H3',
        h4: 'H4',
        h5: 'H5',
        h6: 'H6'
      },
      lists: {
        unordered: 'Liệt kê danh sách',
        ordered: 'Liệt kê theo thứ tự'
      },
      options: {
        help: 'Trợ giúp',
        fullscreen: 'Toàn Màn hình',
        codeview: 'Xem Code'
      },
      paragraph: {
        paragraph: 'Canh lề',
        outdent: 'Dịch sang trái',
        indent: 'Dịch sang phải',
        left: 'Canh trái',
        center: 'Canh giữa',
        right: 'Canh phải',
        justify: 'Canh đều'
      },
      color: {
        recent: 'Màu chữ',
        more: 'Mở rộng',
        background: 'Màu nền',
        foreground: 'Màu chữ',
        transparent: 'trong suốt',
        setTransparent: 'Nền trong suốt',
        reset: 'Thiết lập lại',
        resetToDefault: 'Trở lại ban đầu'
      },
      shortcut: {
        shortcuts: 'Phím tắt',
        close: 'Đóng',
        textFormatting: 'Định dạng Văn bản',
        action: 'Hành động',
        paragraphFormatting: 'Định dạng',
        documentStyle: 'Kiểu văn bản',
        extraKeys: 'Phím bổ sung'
      },
      help: {
        'insertParagraph': 'Chèn văn bản',
        'undo': 'Hoàn tác lệnh cuối cùng',
        'redo': 'Làm lại lệnh cuối cùng',
        'tab': 'Tab',
        'untab': 'Untab',
        'bold': 'Đặt kiểu in đậm',
        'italic': 'Đặt kiểu in nghiêng',
        'underline': 'Đặt kiểu gạch chân',
        'strikethrough': 'Đặt kiểu gạch ngang',
        'removeFormat': 'Xóa định dạng',
        'justifyLeft': 'Căn lề trái',
        'justifyCenter': 'Căn lề giữa',
        'justifyRight': 'Căn lề phải',
        'justifyFull': 'Căn chỉnh đầy đủ',
        'insertUnorderedList': 'Chuyển đổi danh sách không có thứ tự',
        'insertOrderedList': 'Chuyển đổi danh sách theo thứ tự',
        'outdent': 'Giãn lề đoạn hiện tại',
        'indent': 'Thụt lề đoạn hiện tại',
        'formatPara': 'Thay đổi dịnh sạng của block hiện tại thành một đoạn thẻ (Thẻ P)',
        'formatH1': 'Thay đổi dịnh sạng của block hiện tại thành một đoạn thẻ (Thẻ H1)',
        'formatH2': 'Thay đổi dịnh sạng của block hiện tại thành một đoạn thẻ (Thẻ H2)',
        'formatH3': 'Thay đổi dịnh sạng của block hiện tại thành một đoạn thẻ (Thẻ H3)',
        'formatH4': 'Thay đổi dịnh sạng của block hiện tại thành một đoạn thẻ (Thẻ H4)',
        'formatH5': 'Thay đổi dịnh sạng của block hiện tại thành một đoạn thẻ (Thẻ H5)',
        'formatH6': 'Thay đổi dịnh sạng của block hiện tại thành một đoạn thẻ (Thẻ H6)',
        'insertHorizontalRule': 'Chèn đường gạch ngang',
        'linkDialog.show': 'Hiển thị liên kết'
      },
      history: {
        undo: 'Lùi lại',
        redo: 'Làm lại'
      },
      specialChar: {
        specialChar: 'SPECIAL CHARACTERS',
        select: 'Chọn ký tự đặc biệt'
      }
    }
  });
})(jQuery);
