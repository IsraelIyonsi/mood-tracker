<?php

declare(strict_types=1);

namespace MoodTracker\Php;

use PDO;
use Psr\Http\Message\ResponseInterface;
use Psr\Http\Message\ServerRequestInterface;

final class SummaryRoute
{
    private const TABLE = 'MoodEntries';

    public function __construct(private readonly PDO $pdo)
    {
    }

    public function handle(ServerRequestInterface $_request, ResponseInterface $response): ResponseInterface
    {
        $entries = $this->loadEntries();
        $counts = $this->countByMood();

        $html = $this->render($entries, $counts);
        $response->getBody()->write($html);

        return $response->withHeader('Content-Type', 'text/html; charset=utf-8');
    }

    private function loadEntries(): array
    {
        $stmt = $this->pdo->query(
            'SELECT Id, Mood, Note, LoggedAt FROM ' . self::TABLE . ' ORDER BY LoggedAt DESC LIMIT 100'
        );
        return $stmt->fetchAll();
    }

    private function countByMood(): array
    {
        $stmt = $this->pdo->query(
            'SELECT Mood, COUNT(*) AS Total FROM ' . self::TABLE . ' GROUP BY Mood ORDER BY Total DESC'
        );
        $rows = $stmt->fetchAll();
        $result = [];
        foreach ($rows as $row) {
            $result[$row['Mood']] = (int) $row['Total'];
        }
        return $result;
    }

    private function render(array $entries, array $counts): string
    {
        ob_start();
        include __DIR__ . '/../views/summary.phtml';
        return ob_get_clean() ?: '';
    }
}
