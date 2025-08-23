import { Injectable } from '@angular/core';
import { HubApiServiceClient, HubApiServiceDefinition, NotificationEvent, SubscribeRequest } from '../../../generated/hub_api';
import { GrpcService } from './grpc.service';
import { from, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class MessageHubService {
  client: HubApiServiceClient;
  constructor(private readonly grpc: GrpcService) {
    this.client = grpc.getHubClient(HubApiServiceDefinition);
  }

  subscribe(roomId: string): Observable<NotificationEvent> {
    const request = SubscribeRequest.create();
    request.channelId = roomId;
    const call = this.client.subscribe(request);
    return from(call);
  }
}
