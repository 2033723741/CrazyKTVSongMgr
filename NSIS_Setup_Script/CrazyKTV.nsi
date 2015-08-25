; �q�Φw�˦w�˸}��
; �ק�: ken670128
; ����: 2.0
; ���: 2013/05/19
; NSIS: 2.46
; �q�Ϊ�


; �w�q�{���W�٩M�����s��
;========================================================================================================
; �� �{�����W�� (�Y�n��W�٪��ܧ�o��)
!define PRODUCT_NAME "CrazyKTV"

; �� �{�������� (�睊�ɭn��o��)
!define PRODUCT_VERSION "v5.3.9"

; �� �{������
!define PRODUCT_CAT "�C��u��\�d��OK�t��"

; �� �{�������ɤιϥ�
!define PRODUCT_EXE "CrazyKTV.exe"

; �]�w���Y�榡
SetCompressor lzma

; ���J�ۭq�y��
!include "Lang.nsi"

; �w�q������
!define INSTALLER_STD

; �ϥΪ��v��
; RequestExecutionLevel none

; �h��y�����
Unicode true

; 64�줸�t�Τ��
!include "x64.nsh"


; MUI �ɭ��t��
;========================================================================================================
; ���J MUI �t��
!include "MUI2.nsh"

; �]�w MUI �ɭ�
!define MUI_ABORTWARNING
!define MUI_ICON "resource\CrazyKTV.ico"
!define MUI_UNICON "resource\uninst.ico"
!define MUI_HEADERIMAGE
!define MUI_HEADERIMAGE_BITMAP "resource\orange-header.bmp"
!define MUI_UNHEADERIMAGE_BITMAP "resource\orange-header-uninstall.bmp"
!define MUI_WELCOMEFINISHPAGE_BITMAP "resource\orange.bmp"
!define MUI_COMPONENTSPAGE_CHECKBITMAP "resource\plastic.bmp"

; �w�ﭶ��
!insertmacro MUI_PAGE_WELCOME

; �w�ˤ��󭶭�
!insertmacro MUI_PAGE_COMPONENTS

; �w�˸��|����
!insertmacro MUI_PAGE_DIRECTORY

; �w���ɮ׭���
!insertmacro MUI_PAGE_INSTFILES

; �w�˵�������
!insertmacro MUI_PAGE_FINISH

; �Ѱ��w�˽T�{����
!insertmacro MUI_UNPAGE_CONFIRM

; �Ѱ��w���ɮ׭���
!insertmacro MUI_UNPAGE_INSTFILES

; ���J�y��
!insertmacro MUI_LANGUAGE "TradChinese"


; �]�w�w�˶���
;========================================================================================================
; �]�w�w�˵{���W��
Name "${PRODUCT_NAME} ${PRODUCT_VERSION}"

; �]�w�w�˵{����X�ɮ�
!ifdef INSTALLER_STD
	OutFile ".\out\${PRODUCT_NAME}_Full.exe"
!endif

; �]�w�w������
InstType "����w��"

; �]�w�w�˵{���w�]���w�˸��|
InstallDir "$EXEDIR\${PRODUCT_NAME}"

; �w�˥D�{��
	Section "${PRODUCT_NAME} �D�{��" inst1
		SectionIn 1 RO
		SetShellVarContext all
		SetOutPath "$INSTDIR"
		File /r ".\include\*.*"
	SectionEnd

; �w�˨�}�l�\���
	!ifdef INSTALLER_STD
		Section "�ୱ�ζ}�l�\���s��" inst2
			SectionIn 1
			SetShellVarContext all
			SetOutPath "$INSTDIR"
			CreateDirectory "$SMPROGRAMS\${PRODUCT_NAME}"
			CreateShortCut "$SMPROGRAMS\${PRODUCT_NAME}\${PRODUCT_NAME}.lnk" "$INSTDIR\${PRODUCT_EXE}"
			CreateShortCut "$SMPROGRAMS\${PRODUCT_NAME}\�Ѱ��w�� ${PRODUCT_NAME}.lnk" "$INSTDIR\Uninstall.exe"
			CreateShortCut "$DESKTOP\${PRODUCT_NAME}.lnk" "$INSTDIR\${PRODUCT_EXE}"
		SectionEnd
	!endif



; �n��ӧO�]�w
  Section "" soft
        ; �n��
        ; WriteRegDWORD HKCU "" "" 0x00000000
  SectionEnd	

  
; �n���]�w
	Section "" end
		; �n���Ѱ��w�˪����
        WriteRegStr HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}" "DisplayName" "${PRODUCT_NAME}"
        WriteRegStr HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}" "DisplayIcon" "$INSTDIR\${PRODUCT_EXE}"
        WriteRegStr HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}" "UninstallString" "$\"$INSTDIR\Uninstall.exe$\""
        WriteRegStr HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}" "Installer Language" "$LANGUAGE"
        WriteUninstaller "$INSTDIR\Uninstall.exe"
	SectionEnd


; �]�w�w�˶��ت��y�z
	!insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
		!insertmacro MUI_DESCRIPTION_TEXT ${inst1} "�w�� ${PRODUCT_NAME} �D�{����A���q���ءC"
		!insertmacro MUI_DESCRIPTION_TEXT ${inst2} "�[�J�}�l�\���i�H���A���K������ ${PRODUCT_NAME} �θѰ��w�ˡC"
	!insertmacro MUI_FUNCTION_DESCRIPTION_END


; �]�w�Ѱ��w�˶���
;========================================================================================================
	Section Uninstall
		; �n�R�����ɮשM��Ƨ�
		SetShellVarContext all
		!ifdef INSTALLER_STD
			RMDir /r "$SMPROGRAMS\${PRODUCT_NAME}"
		!endif
		
		Delete /REBOOTOK "$DESKTOP\${PRODUCT_NAME}.lnk"
		RMDir /r "$INSTDIR"
		
		; �R���n�����X
		${If} ${RunningX64}
			DeleteRegKey HKCU "Software\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}"
		${Else}
			DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}"
		${EndIf}
	SectionEnd