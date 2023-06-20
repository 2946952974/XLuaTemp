Prefabes={}


-- 预制体加载路径
function Prefabes:Load(path)
	local prefabe = CS.UnityEngine.Resources.Load(path)
	local go  = CS.UnityEngine.Object.Instantiate(prefabe)
	go.name=prefabe.name
	local canvas=CS.UnityEngine.GameObject.Find("/Canvas").transform

	local trs=go.transform
	trs:SetParent(canvas)
	trs.localPosition=CS.UnityEngine.Vector3.zero
	trs.localPosition=CS.UnityEngine.Quaternion.identity
	trs.localScale=CS.UnityEngine.Vector3.one

	trs.offsetMin=CS.UnityEngine.Vector2.zero
	trs.offsetMax=CS.UnityEngine.Vector2.zero
	return go
end