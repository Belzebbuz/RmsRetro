import { inject, Injectable } from '@angular/core';
import {
  Client,
  ClientMiddlewareCall,
  CompatServiceDefinition,
  createChannel,
  createClientFactory,
  Metadata,
} from 'nice-grpc-web';
import { CallOptions, ClientMiddleware } from 'nice-grpc-common';
import { environment } from '../../../environments/environment';
import { MetadataService } from '../metadata.service';
import { TuiAlertService } from '@taiga-ui/core';
/**
 * Сервис для создания клиентов gRPC с поддержкой middleware-обработчиков.
 * Использует настроенную конфигурацию канала и middleware для аутентификации и обработки ошибок.
 */
@Injectable({
  providedIn: 'root',
})
export class GrpcService {
  /**
   * Базовый канал для соединения с gRPC-сервером.
   * Инициализируется через базовый URL из окружения.
   */
  apiChannel = createChannel(environment.baseGrpcUrl);
  hubChannel = createChannel(environment.baseHubUrl);

  /**
   * Сервис для работы с метаданными (например, получение ID игрока).
   */
  metadata = inject(MetadataService);

  /**
   * Фабрика клиентов с применёнными middleware-обработчиками:
   * - `authMiddleware` для добавления токена аутентификации
   * - `erroHandlingMiddleware` для логирования ошибок
   */
  clientFactory = createClientFactory()
    .use(this.authMiddleware.bind(this))
    .use(this.erroHandlingMiddleware.bind(this));

  private readonly alerts = inject(TuiAlertService);

  /**
   * Создаёт клиентский объект для работы с конкретным gRPC-api-сервисом.
   * @param definition Определение сервиса из protobuf-дескриптора
   * @param middleware Дополнительный middleware (опционально)
   * @returns Инстанс клиента с настроенными middleware
   */
  public getApiClient<Service extends CompatServiceDefinition>(
    definition: Service,
    middleware?: ClientMiddleware
  ): Client<Service> {
    if (middleware) return this.clientFactory.use(middleware).create(definition, this.apiChannel);
    return this.clientFactory.create(definition, this.apiChannel);
  }

  /**
   * Создаёт клиентский объект для работы с конкретным gRPC-hub-сервисом.
   * @param definition Определение сервиса из protobuf-дескриптора
   * @param middleware Дополнительный middleware (опционально)
   * @returns Инстанс клиента с настроенными middleware
   */
  public getHubClient<Service extends CompatServiceDefinition>(
    definition: Service,
    middleware?: ClientMiddleware
  ): Client<Service> {
    if (middleware) return this.clientFactory.use(middleware).create(definition, this.apiChannel);
    return this.clientFactory.create(definition, this.hubChannel);
  }
  /**
   * Middleware для добавления токена аутентификации в запросы.
   * Использует ID игрока из сервиса метаданных.
   * @param call Объект вызова gRPC-метода
   * @param options Параметры вызова (включая метаданные)
   * @yield Результат выполнения следующего middleware в цепочке
   */
  async *authMiddleware<Request, Response>(
    call: ClientMiddlewareCall<Request, Response>,
    options: CallOptions
  ) {
    const playerMetadata = this.metadata.getPlayerId();

    return yield* call.next(call.request, {
      ...options,
      metadata: Metadata(options.metadata).set('x-player-id', playerMetadata ?? ''),
    });
  }

  /**
   * Middleware для обработки ошибок в gRPC-запросах.
   * Перехватывает исключения, отображает уведомление об ошибке через сервис `alerts`
   * и пробрасывает ошибку дальше по цепочке middleware.
   * @param call Объект вызова gRPC-метода
   * @param options Параметры вызова (включая метаданные)
   * @yield Результат выполнения следующего middleware в цепочке
   */
  async *erroHandlingMiddleware<Request, Response>(
    call: ClientMiddlewareCall<Request, Response>,
    options: CallOptions
  ) {
    try {
      return yield* call.next(call.request, options);
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : 'Неизвестная ошибка сервера';

      this.alerts
        .open(errorMessage, { appearance: 'negative', label: 'Ошибка сервера' })
        .subscribe();
      throw error;
    }
  }
}
export type Middleware = AsyncGenerator<Awaited<Response>, void | Awaited<Response>, undefined>;
