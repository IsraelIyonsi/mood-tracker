<?php

declare(strict_types=1);

require __DIR__ . '/../vendor/autoload.php';

use MoodTracker\Php\SqliteConnection;
use MoodTracker\Php\SummaryRoute;
use Slim\Factory\AppFactory;

$databasePath = getenv('MOOD_DB_PATH') ?: '/data/mood.db';

$pdo = SqliteConnection::tryOpen($databasePath);

$app = AppFactory::create();
$app->addRoutingMiddleware();
$app->addErrorMiddleware(true, true, true);

$summaryRoute = new SummaryRoute($pdo);
$app->get('/summary', [$summaryRoute, 'handle']);

$app->run();
