protoc --plugin=protoc-gen-ts_proto=node_modules\.bin\protoc-gen-ts_proto.cmd ^
--ts_proto_out=./src/generated ^
--ts_proto_opt="env=browser,outputServices=nice-grpc,outputServices=generic-definitions,outputJsonMethods=false,useExactTypes=false" ^
--proto_path="../hub/RmsRetro.MessagHub.Protos/" ^
"../hub/RmsRetro.MessagHub.Protos/hub_api.proto"