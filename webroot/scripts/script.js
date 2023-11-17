const source = new EventSource("http://localhost:20000/sse/test")
source.onmessage = (event) => console.log("SSE event", event.data)

const btn1 = document.getElementById("button")
const btn2 = document.getElementById("button2")
const btnDevTools = document.getElementById("buttonDevTools")
const dr = document.getElementById("Drag")

btnDevTools.onclick = () => webViewShowDevTools()

btn1.onclick = async () => {
    var res = await webViewRequest("cmd1", {
        text: "Text",
        id: 123
    })
    console.log("cmd1", res)
}

webViewRegisterDragEnd(() => console.log("Dräg ended"))

const onDragStart = evt => { 
    webViewDragStart(["file:///home/uwe/test"])
    evt.preventDefault()
}

dr.onmousedown = onDragStart


