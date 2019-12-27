$.Pgater = (function() {
  var agent = navigator.userAgent.toLowerCase();
  var iswx = agent.indexOf("qqbrowser") >= 0;
  if (iswx) {
    var File = $(
      "<input type='file' id='csl_gater_file' accept='image/*' capture='camera' multiple='multiple'>"
    );
  } else {
    var File = $(
      "<input type='file' id='csl_gater_file' accept='image/*' multiple='multiple'>"
    );
  }
  File.css("display", "none");
  return function(target, callBack) {
    console.log(File);
    this.ele = File;
    this.parent = target;
    this.parent.append(this.ele);
    this.bindClk(this.parent, this.ele[0]);
    this.bindFuc(this.ele, callBack);
  };
})();
$.Pgater.prototype.bindFuc = function(ele, callBack) {
  ele.on("change", function() {
	console.log('ele[0]: ');
	console.log(ele[0]);
	console.log('ele[0].files: ');
    console.log(ele[0].files);
    let t = 137;
    let systemResource = 136;
    var all = ele[0].files;
    // let dataReq = { type: t, systemResource: systemResource, file: all };

	var formData = new FormData();
	formData.append('file', ele[0].files[0]);
	formData.append('type', t);
  formData.append('systemResource', systemResource);
  let token = window.sessionStorage.getItem('token')
	$.ajax({
    type:"POST", 
    headers: {
      Authorization: `Bearer ${token}`
    },                            
		url:"http://10.89.36.154:5801/eqpapi/Upload",                  
		data: formData  ,                    
		async: true,
		cache: false,
		contentType: false,
		processData: false,
		success: function (ret) {                 
      console.log("ajax请求成功")
      console.log(ret)
		},
		//请求失败触发的方法
		error:function(XMLHttpRequest, textStatus, errorThrown){
			console.log("ajax请求失败");
      console.log("请求对象XMLHttpRequest: ");
      console.log(XMLHttpRequest);
			console.log("错误类型textStatus: "+textStatus);
			console.log("异常对象errorThrown: "+errorThrown);
		}
	})
    var reader = new FileReader();
    var album = [];
    console.log(all.length);
    var length = all.length;
    var i = 0;
    var recur = function() {
      console.log(all[i]);
      reader.readAsDataURL(all[i]);
      var One = all[i];
      reader.onload = function(e) {
        //alert(One);
        console.log(One);
        One.data = this.result;
        album.push(One);
        i++;
        if (i < length) {
          recur();
        } else {
          ele.value = "";
          // alert(i);
          // callBack(album,img);
          callBack(album);
        }
      };
    };
    recur();
  });
};
$.Pgater.prototype.bindClk = function(ele, tar) {
  ele.on("click", function() {
    tar.click();
  });
};
