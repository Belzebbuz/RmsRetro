import { inject, Injectable } from '@angular/core';
import { ApiServiceClient, ApiServiceDefinition } from '../../../generated/api';
import { GrpcService } from './grpc.service';
import { Empty } from '../../../generated/google/protobuf/empty';
import { MetadataService } from '../metadata.service';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  client: ApiServiceClient;
  metadata = inject(MetadataService);
  constructor(private readonly grpc: GrpcService) {
    this.client = grpc.getApiClient(ApiServiceDefinition);
  }

  async activateUser() {
    const response = await this.client.activateUser(Empty.create());
    this.metadata.storePlayerId(response.userId);
  }
}
