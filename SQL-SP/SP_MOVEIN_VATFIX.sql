USE [YZ]
GO
/****** Object:  StoredProcedure [dbo].[SP_MOVEIN_VATFIX]    Script Date: 12/02/2025 09:57:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<RcBuilder>
-- Create date: <2025-02-04>

-- SP_MOVEIN_CURRENT	
-- SELECT * FROM MOVEIN
-- SELECT Vat_amount, Bill_ID, Bill_M_ID, * FROM Bill_M WHERE BILL_M_ID = 42766
-- =============================================
ALTER PROCEDURE [dbo].[SP_MOVEIN_VATFIX] 	
AS
BEGIN	
	SET NOCOUNT ON;	

	-- bill_m_info.aspx?BILL_ID=42426
	-- DEFAULT = 18
	UPDATE t1
	SET		t1.[קוד_סוג_תנועה] = 'חשט'
	FROM	MOVEIN t1	
	WHERE	t1.[קוד_סוג_תנועה] = 'חשע'

	UPDATE	t1
	SET		t1.[קוד_סוג_תנועה] = 'חשע'
	FROM	MOVEIN t1
	JOIN	Bill_M t2 ON t1.[אסמכתא_ראשונה] = t2.BILL_M_ID
	WHERE	t2.Vat_amount = 17  -- SUPPORT 17
	AND		t1.[קוד_סוג_תנועה] = 'חשט'

	--

	-- BILL_R_INFO.aspx?BILL_ID=3241
	-- DEFAULT = 18
	UPDATE t1
	SET		t1.[קוד_סוג_תנועה] = 'חז8'
	FROM	MOVEIN t1	
	WHERE	t1.[קוד_סוג_תנועה] = 'חז'

	UPDATE t1
	SET		t1.[קוד_סוג_תנועה] = 'חז'
	FROM	MOVEIN t1
	JOIN	BILL_R t2 ON t1.[אסמכתא_ראשונה] = t2.BILL_R_ID
	WHERE	t2.Vat_amount = 17  -- SUPPORT 17
	AND		t1.[קוד_סוג_תנועה] = 'חז8'

	--

	-- Check_info.aspx?type=a&Check_id=32132
	-- artist_INFO.aspx?artist_code=1442
	-- SELECT ARTIST_CODE, Artist_Check_id,* FROM Artist_Checks WHERE Artist_Check_NUM = 32124	
	-- DEFAULT = 18
	UPDATE t1
	SET		t1.[קוד_סוג_תנועה] = 'חסט'
	FROM	MOVEIN t1	
	WHERE	t1.[קוד_סוג_תנועה] = 'חשס'

	UPDATE t1
	SET		t1.[קוד_סוג_תנועה] = 'חשס'
	FROM	MOVEIN t1
	JOIN	Artist_Checks t2 ON (
		t2.Artist_Check_NUM LIKE '%' + LTRIM(RTRIM(t1.[אסמכתא_ראשונה]))
		AND t2.Income_Bill_Num LIKE '%' + LTRIM(RTRIM(t1.[אסמכתא_שניה]))	
	)
	WHERE	t2.Vat = 17 -- SUPPORT 17
	AND		t1.[קוד_סוג_תנועה] = 'חסט'

	--

	-- CONSTANT 18
	UPDATE t1
	SET		t1.[קוד_סוג_תנועה] = 'תשט'
	FROM	MOVEIN t1	
	WHERE	t1.[קוד_סוג_תנועה] = 'תשס'

	--

	-- CONSTANT 18
	UPDATE t1
	SET		t1.[קוד_סוג_תנועה] = 'ח18'
	FROM	MOVEIN t1	
	WHERE	t1.[קוד_סוג_תנועה] = 'חסח'

	--

	-- splr_INFO.aspx?splr_id=17
	-- SELECT Vat, * FROM SPLR_CHECK WHERE SPLR_Check_NUM = 7908 
	-- DEFAULT = 18
	UPDATE t1
	SET		t1.[קוד_סוג_תנועה] = 'תש8'
	FROM	MOVEIN t1	
	WHERE	t1.[קוד_סוג_תנועה] = 'תש'

	UPDATE t1
	SET		t1.[קוד_סוג_תנועה] = 'תש'
	FROM	MOVEIN t1
	JOIN	SPLR_CHECK t2 ON t1.[אסמכתא_ראשונה] = t2.SPLR_Check_NUM AND t1.[אסמכתא_שניה] = t2.Income_bill_num
	WHERE	t2.Vat = 17  -- SUPPORT 17
	AND		t1.[קוד_סוג_תנועה] = 'תש8'

	--

	-- BILL_SELF_INFO.aspx?BILL_ID=2161
	-- SELECT Maam, * FROM BILL_SELF WHERE BILL_SELF_ID = 2161	
	-- DEFAULT = 18
	UPDATE t1
	SET		t1.[קוד_סוג_תנועה] = 'חל8'
	FROM	MOVEIN t1	
	WHERE	t1.[קוד_סוג_תנועה] = 'חל7'

	UPDATE t1
	SET		t1.[קוד_סוג_תנועה] = 'חל7'
	FROM	MOVEIN t1
	JOIN	BILL_SELF t2 ON t1.[אסמכתא_ראשונה] = t2.BILL_SELF_ID
	WHERE	t2.Maam = 17  -- SUPPORT 17
	AND		t1.[קוד_סוג_תנועה] = 'חל8'


END
